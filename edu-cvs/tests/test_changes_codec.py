from typing import List, TextIO, Tuple
from unittest import TestCase, main as unittest_main
from os import (
    pardir as os_pardir,
    mkdir as os_mkdir,
    remove as os_remove)
from os.path import (
    join as os_path_join,
    dirname as os_path_dirname,
    abspath as os_path_abspath,
    exists as os_path_exists
)
from sys import path as sys_path
from shutil import rmtree as shutil_rmtree

sys_path.append(os_path_join(
    os_path_dirname(os_path_abspath(__file__)),
    os_pardir))

from cvs.changes_codec import (
    ChangesCodec, ProgramDataFormatError, CodecException)


class GetLastCommitNumber(TestCase):
    _changes_codec: ChangesCodec = ChangesCodec()

    def setUp(self) -> None:
        os_mkdir(self._changes_codec._CVS_DIR_PATH)
        os_mkdir(self._changes_codec._COMMITTED_PATH)

    def tearDown(self) -> None:
        if os_path_exists(self._changes_codec._CVS_DIR_PATH):
            shutil_rmtree(self._changes_codec._CVS_DIR_PATH)

    def test_no_commits(self):
        self.assertEqual(
            self._changes_codec._get_last_commit_number(),
            0)

    def test_only_commit_files(self):
        self._create_files_in_committed_dir(['3', '5', '7'])

        self.assertEqual(
            self._changes_codec._get_last_commit_number(),
            7)

    def test_not_parsing_to_int_file_name(self):
        self._create_files_in_committed_dir(['3', '5', 'asd'])

        self.assertEqual(
            self._changes_codec._get_last_commit_number(),
            5)

    def test_folder_in_committed_dir(self):
        self._create_files_in_committed_dir(['3', '5'])
        self._create_folder_in_committed_dir('231')

        self.assertEqual(
            self._changes_codec._get_last_commit_number(),
            5)

    def test_no_committed_dir(self):
        shutil_rmtree(self._changes_codec._CVS_DIR_PATH)

        with self.assertRaises(OSError) as occurred_err:
            self._changes_codec._get_last_commit_number()

        self.assertEqual(
            occurred_err.exception.args[0],
            'Cannot scan "committed/" for last commit number')

    def test_some_commits_with_garbage_in_committed_dir(self):
        self._create_files_in_committed_dir(['3', '5', 'asd', 'sadas', 'bb'])

        self._create_folder_in_committed_dir('231')
        self._create_folder_in_committed_dir('23')
        self._create_folder_in_committed_dir('21')

        self.assertEqual(
            self._changes_codec._get_last_commit_number(),
            5)

    def _create_files_in_committed_dir(self, filenames: List[str]):
        for filename in filenames:
            full_filename: str = os_path_join(
                self._changes_codec._COMMITTED_PATH,
                filename)

            open(full_filename, 'w').close()

    def _create_folder_in_committed_dir(self, folder_name: str):
        full_folder_name: str = os_path_join(
            self._changes_codec._COMMITTED_PATH,
            folder_name)

        os_mkdir(full_folder_name)


class EncodingChangesTests(TestCase):
    _changes_codec: ChangesCodec = ChangesCodec()

    def test_no_additions(self):
        self._changes_codec._encode_changes(
            ChangesCodec.RepositoryChanges(),
            _TEMP_PATH_1)

        encoded_changes: TextIO = open(_TEMP_PATH_1, 'r')

        self.assertEqual(encoded_changes.read(), '')

    def test_some_additions(self):
        repo_changes: ChangesCodec.RepositoryChanges = (
            ChangesCodec.RepositoryChanges())

        repo_changes.addition = ['123', '321']

        self._changes_codec._encode_changes(
            repo_changes,
            _TEMP_PATH_1)

        encoded_changes: TextIO = open(_TEMP_PATH_1, 'r')

        self.assertEqual(encoded_changes.readline(), 'addition:\n')
        self.assertEqual(encoded_changes.readline(), '123\n')
        self.assertEqual(encoded_changes.readline(), '321\n')
        self.assertEqual(encoded_changes.readline(), '')


class DecodingChangesTests(TestCase):
    _changes_codec: ChangesCodec = ChangesCodec()

    def test_some_additions(self):
        with open(_TEMP_PATH_1, 'w') as encoded_changes:
            encoded_changes.writelines([
                'addition:\n',
                '123\n',
                '321\n'])

        repo_changes: ChangesCodec.RepositoryChanges = (
            self._changes_codec._decode_changes(_TEMP_PATH_1))

        self.assertEqual(len(repo_changes.addition), 2)

        self.assertEqual(repo_changes.addition, ['123', '321'])

    def test_no_changes(self):
        open(_TEMP_PATH_1, 'w').close()

        test_changes: ChangesCodec.RepositoryChanges = (
            self._changes_codec._decode_changes(_TEMP_PATH_1))

        self.assertEqual(len(test_changes.addition), 0)

    def test_no_additions_incorrect_format(self):
        with open(_TEMP_PATH_1, 'w') as encoded_changes:
            encoded_changes.write('addition:\n')

        with self.assertRaises(ProgramDataFormatError) as occurred_err:
            self._changes_codec._decode_changes(_TEMP_PATH_1)

        self.assertEqual(len(occurred_err.exception.args), 1)

        self.assertEqual(
            occurred_err.exception.args[0],
            'No filenames after addition sync pattern')

        self.assertEqual(
            getattr(occurred_err.exception, 'filename', None),
            _TEMP_PATH_1)

    def test_no_addition_sync_pattern(self):
        with open(_TEMP_PATH_1, 'w') as encoded_changes:
            encoded_changes.write('123\n')

        with self.assertRaises(ProgramDataFormatError) as occurred_err:
            self._changes_codec._decode_changes(_TEMP_PATH_1)

        self.assertEqual(len(occurred_err.exception.args), 1)

        self.assertEqual(
            occurred_err.exception.args[0],
            'Addition sync pattern lost')

        self.assertEqual(
            getattr(occurred_err.exception, 'filename', None),
            _TEMP_PATH_1)

    def test_empty_filename(self):
        with open(_TEMP_PATH_1, 'w') as encoded_changes:
            encoded_changes.writelines([
                'addition:\n',
                '123\n',
                '\n'])

        with self.assertRaises(ProgramDataFormatError) as occurred_err:
            self._changes_codec._decode_changes(_TEMP_PATH_1)

        self.assertEqual(len(occurred_err.exception.args), 1)

        self.assertEqual(
            occurred_err.exception.args[0],
            'Got empty filename when decoding changes file')

        self.assertEqual(
            getattr(occurred_err.exception, 'filename', None),
            _TEMP_PATH_1)

    def test_unexpected_end_of_file(self):
        with open(_TEMP_PATH_1, 'w') as encoded_changes:
            encoded_changes.writelines([
                'addition:\n',
                '123\n',
                '555'])

        with self.assertRaises(ProgramDataFormatError) as occurred_err:
            self._changes_codec._decode_changes(_TEMP_PATH_1)

        self.assertEqual(len(occurred_err.exception.args), 1)

        self.assertEqual(
            occurred_err.exception.args[0],
            'Unexpected end of file')

        self.assertEqual(
            getattr(occurred_err.exception, 'filename', None),
            _TEMP_PATH_1)

    @classmethod
    def tearDownClass(cls) -> None:
        os_remove(_TEMP_PATH_1)


class InitTests(TestCase):
    _changes_codec: ChangesCodec = ChangesCodec()

    def test_init(self):
        self._changes_codec.cvs_init()

        self.assertTrue(os_path_exists(self._changes_codec._CVS_DIR_PATH))

        # self.assertTrue(os_path_exists(self._changes_codec._COMMITTED_PATH))

    @classmethod
    def tearDownClass(cls) -> None:
        shutil_rmtree(cls._changes_codec._CVS_DIR_PATH)


class AddTests(TestCase):
    _changes_codec: ChangesCodec = ChangesCodec()

    @classmethod
    def setUpClass(cls) -> None:
        cls._changes_codec.cvs_init()

    def tearDown(self) -> None:
        if os_path_exists(self._changes_codec._STAGED_PATH):
            os_remove(self._changes_codec._STAGED_PATH)

        if os_path_exists(_TEMP_PATH_1):
            os_remove(_TEMP_PATH_1)

        if os_path_exists(_TEMP_PATH_2):
            os_remove(_TEMP_PATH_2)

    def test_no_edu_cvs_dir(self):
        shutil_rmtree(self._changes_codec._CVS_DIR_PATH)

        with self.assertRaises(CodecException) as exc_handle:
            self._changes_codec.cvs_add(_TEMP_PATH_1)

        self.assertEqual(exc_handle.exception.args, ('add: init repo first',))

        self._changes_codec.cvs_init()

    def test_does_not_exist(self):
        cvs_add_result: Tuple[str, str] = self._changes_codec.cvs_add(
            _TEMP_PATH_1)

        self.assertEqual(cvs_add_result, ('does not exist', _TEMP_PATH_1))

        self.assertFalse(os_path_exists(self._changes_codec._STAGED_PATH))

    def test_not_a_file(self):
        os_mkdir(_TEMP_PATH_1)

        cvs_add_result: Tuple[str, str] = self._changes_codec.cvs_add(
            _TEMP_PATH_1)

        self.assertEqual(cvs_add_result, ('not a file', _TEMP_PATH_1))

        self.assertFalse(os_path_exists(self._changes_codec._STAGED_PATH))

        shutil_rmtree(_TEMP_PATH_1)

    def test_already_added(self):
        open(_TEMP_PATH_1, 'w').close()

        self._changes_codec.cvs_add(_TEMP_PATH_1)
        cvs_add_result: Tuple[str, str] = self._changes_codec.cvs_add(
            _TEMP_PATH_1)

        self.assertEqual(
            ('already added', _TEMP_PATH_1),
            cvs_add_result)

        with open(self._changes_codec._STAGED_PATH) as staged_handle:
            self.assertEqual('addition:\n', staged_handle.readline())
            self.assertEqual(
                _TEMP_PATH_1 + '\n',
                staged_handle.readline())
            self.assertEqual('', staged_handle.readline())

    def test_success_no_existing_staged_file(self):
        open(_TEMP_PATH_1, 'w')
        open(_TEMP_PATH_2, 'w')

        cvs_add_result: Tuple[str, str] = self._changes_codec.cvs_add(
            _TEMP_PATH_1)

        self.assertEqual(('success', _TEMP_PATH_1), cvs_add_result)

        cvs_add_result = self._changes_codec.cvs_add(_TEMP_PATH_2)

        self.assertEqual(('success', _TEMP_PATH_2), cvs_add_result)

        with open(self._changes_codec._STAGED_PATH) as staged_handle:
            self.assertEqual('addition:\n', staged_handle.readline())
            self.assertEqual(
                _TEMP_PATH_1 + '\n',
                staged_handle.readline())
            self.assertEqual(
                _TEMP_PATH_2 + '\n', staged_handle.readline())
            self.assertEqual('', staged_handle.readline())

    def test_success_with_existing_staged_file(self):
        open(_TEMP_PATH_1, 'w')
        open(_TEMP_PATH_2, 'w')

        with open(self._changes_codec._STAGED_PATH, 'w') as staged_handle:
            staged_handle.writelines([
                'addition:\n',
                _TEMP_PATH_1 + '\n'])

        cvs_add_result = self._changes_codec.cvs_add(_TEMP_PATH_2)

        self.assertEqual(('success', _TEMP_PATH_2), cvs_add_result)

        with open(self._changes_codec._STAGED_PATH) as staged_handle:
            self.assertEqual('addition:\n', staged_handle.readline())
            self.assertEqual(
                _TEMP_PATH_1 + '\n', staged_handle.readline())
            self.assertEqual(
                _TEMP_PATH_2 + '\n', staged_handle.readline())
            self.assertEqual('', staged_handle.readline())

    @classmethod
    def tearDownClass(cls) -> None:
        shutil_rmtree(cls._changes_codec._CVS_DIR_PATH)


_TEMP_PATH_1 = 'temp_1'
_TEMP_PATH_2 = 'temp_2'


if __name__ == '__main__':
    unittest_main()
