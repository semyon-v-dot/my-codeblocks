from typing import Tuple, List, TextIO, Iterator
from os import mkdir as os_mkdir, scandir as os_scandir, DirEntry
from os.path import (
    exists as os_path_exists,
    join as os_path_join,
    isfile as os_path_isfile)

from cvs import ProgramException


class CodecException(ProgramException):
    """For all codec-related exceptions"""
    filename: str

    @classmethod
    def with_filename(cls, *args, filename: str = ''):
        """Returns new instance of class with 'filename' field set"""
        output_exc: CodecException = cls(*args)

        output_exc.filename = filename

        return output_exc


class ProgramDataFormatError(CodecException):
    """Raises when program data format is incorrect"""


class ChangesCodec:
    """Codec of repository changes

    Decodes and encodes staged or committed repository changes

    Note that current working dir is considered as root for all paths
    in this class
    """
    class RepositoryChanges:
        # none: List[str]
        addition: List[str]
        # deletion: List[str]

        def __init__(self):
            self.addition = []

    # WouldBeBetter: Add field for current dir for easier testing. Use not
    #  just current working dir
    _CVS_DIR_PATH: str = '.edu-cvs'
    _COMMITTED_PATH: str = os_path_join('.edu-cvs', 'committed')
    _STAGED_PATH: str = os_path_join('.edu-cvs', 'staged')

    def cvs_init(self):
        """Initializes edu-cvs repository in current working directory"""
        try:
            if not os_path_exists(self._CVS_DIR_PATH):
                os_mkdir(self._CVS_DIR_PATH)

            # if not os_path_exists(self._COMMITTED_PATH):
            #     os_mkdir(self._COMMITTED_PATH)

        except OSError as occurred_err:
            raise OSError(
                'init: cannot create folder', *occurred_err.args)

    def cvs_add(self, input_path: str) -> Tuple[str, str]:
        """Adds single file to staged changes

        Returns pair: result status AND filename

        Possible return statuses:
            'does not exist'
            'success'
            'not a file'
            'already added'
        """

        if not os_path_exists(self._CVS_DIR_PATH):
            raise CodecException('add: init repo first')

        if not os_path_exists(input_path):
            return 'does not exist', input_path

        elif os_path_isfile(input_path):
            repo_changes: 'ChangesCodec.RepositoryChanges'

            if os_path_exists(self._STAGED_PATH):
                repo_changes = self._decode_changes(self._STAGED_PATH)
            else:
                repo_changes = self.RepositoryChanges()

            if input_path in repo_changes.addition:
                return 'already added', input_path

            repo_changes.addition.append(input_path)

            self._encode_changes(repo_changes, self._STAGED_PATH)

            return 'success', input_path

        else:
            return 'not a file', input_path

    def _get_last_commit_number(self) -> int:
        """Returns last commit number

        Committed files' changes have names of their commitment numbers. '1',
        '2' etc.

        Returns 0 if there are no commit files
        """
        commit_numbers: List[int] = []
        commit_dir: Iterator[DirEntry]

        try:
            with os_scandir(self._COMMITTED_PATH) as commit_dir:
                for dir_entry in commit_dir:
                    if dir_entry.is_file():
                        try:
                            commit_numbers.append(int(dir_entry.name))
                        except ValueError as occurred_err:
                            if occurred_err.args[0].startswith(
                                    'invalid literal for int() with base 10:'):
                                pass
                            else:
                                raise
        except OSError as occurred_err:
            raise OSError(
                'Cannot scan "committed/" for last commit number',
                *occurred_err.args)

        if len(commit_numbers) == 0:
            return 0
        else:
            return max(commit_numbers)

    def _decode_changes(
            self,
            filename: str) -> 'ChangesCodec.RepositoryChanges':
        """Decodes RepositoryChanges instance from a file"""
        file_with_changes: TextIO
        repo_changes: 'ChangesCodec.RepositoryChanges' = (
            self.RepositoryChanges())

        try:
            with open(filename, 'r') as file_with_changes:
                addition_sync_pattern: str = file_with_changes.readline()

                # No changes
                if addition_sync_pattern == '':
                    return repo_changes

                if addition_sync_pattern != 'addition:\n':
                    raise ProgramDataFormatError.with_filename(
                        'Addition sync pattern lost', filename=filename)

                adding_filename: str = file_with_changes.readline()

                if adding_filename == '':
                    raise ProgramDataFormatError.with_filename(
                        'No filenames after addition sync pattern',
                        filename=filename)

                while adding_filename != '':
                    if adding_filename == '\n':
                        raise ProgramDataFormatError.with_filename(
                            'Got empty filename when decoding changes file',
                            filename=filename)

                    if adding_filename[-1] != '\n':
                        raise ProgramDataFormatError.with_filename(
                            'Unexpected end of file',
                            filename=filename)

                    repo_changes.addition.append(adding_filename[:-1])

                    adding_filename = file_with_changes.readline()

        except OSError as occurred_err:
            raise OSError(
                'Cannot decode changes from given file', *occurred_err.args)

        return repo_changes

    def _encode_changes(
            self,
            repo_changes: 'ChangesCodec.RepositoryChanges',
            filename: str):
        """Encodes repository changes into a file"""
        changes_file: TextIO

        try:
            with open(filename, 'w') as changes_file:
                if len(repo_changes.addition) != 0:
                    changes_file.write('addition:\n')

                    for change in repo_changes.addition:
                        changes_file.write(change + '\n')
        except OSError as occurred_err:
            raise OSError('Cannot encode changes', *occurred_err.args)
