from typing import List, Set, Tuple
from os import getcwd as os_getcwd

from cvs import ProgramException
from .changes_codec import ChangesCodec


class CVSException(ProgramException):
    """For all CVS-related exceptions"""


class ConsoleInputError(CVSException):
    """Raises when input console arguments are incorrect"""


class ControlVersionSystem:
    """Main CVS class"""
    _CVS_COMMANDS: Set[str] = {'init', 'add'}

    _cvs_command: str
    _cvs_command_args: List[str]

    _changes_codec: ChangesCodec

    def __init__(self, input_command_with_args: List[str]):
        self._cvs_command = input_command_with_args[0]
        self._cvs_command_args = input_command_with_args[1:]

        self._changes_codec = ChangesCodec()

    def process_cvs_command(self) -> str:
        """Processes input arguments

         Returns info message in case of success
         """
        if self._cvs_command not in self._CVS_COMMANDS:
            raise ConsoleInputError('Unknown command')

        elif self._cvs_command == 'init':
            self._changes_codec.cvs_init()

            # WouldBeBetter: Change 'os_getcwd' to specific dir
            return (
                "Initialized Edu-cvs repository in " + os_getcwd())

        elif self._cvs_command == 'add':
            if len(self._cvs_command_args) == 0:
                raise ConsoleInputError('No input for "add"')

            if len(self._cvs_command_args) > 1:
                raise ConsoleInputError('add: multiple files')

            cvs_add_result: Tuple[str, str] = (
                self._changes_codec.cvs_add(self._cvs_command_args[0]))

            return '\n'.join(
                [item[0] + ': ' + item[1] for item in [cvs_add_result]])

        raise CVSException('[_cvs_command] switch was passed')
