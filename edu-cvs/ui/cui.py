from configparser import (
    ConfigParser,
    ParsingError as configparser_ParsingError)
from sys import exit as sys_exit
from argparse import ArgumentParser, Namespace, RawDescriptionHelpFormatter
from os.path import split as os_path_split, join as os_path_join

from cvs import ProgramException
from cvs.cvs_main import ControlVersionSystem, ConsoleInputError
from cvs.changes_codec import ProgramDataFormatError, CodecException


def get_current_version() -> str:
    """Gives current version str

    Gives version based on 'config.ini' in application's folder. If data is
    corrupted then "Cannot read 'config.ini' file" str returns
    """
    config: ConfigParser = ConfigParser()

    current_dir: str
    parent_dir: str

    current_dir, _ = os_path_split(__file__)
    parent_dir, _ = os_path_split(current_dir)

    try:
        config.read(os_path_join(parent_dir, 'config.ini'))
    except configparser_ParsingError:
        pass

    if 'VERSION' not in config or 'current_version' not in config['VERSION']:
        return "Cannot read 'config.ini' file"
    else:
        return config['VERSION']['current_version']


def exit_with_exception(
        info_for_user: str,
        input_exception: Exception,
        debugging_mode: bool):
    """Exits with exception

    Prints info to stdout for user and to stderr for debugging. Instead of
    error codes, 'error strings' are used. 'Error strings' have format:
    '\n' + (exception name) + ': ' + (exception args)
    """
    print(info_for_user)

    if debugging_mode:
        sys_exit(
            "\n"
            + input_exception.__class__.__name__
            + ": "
            + str(input_exception))
    else:
        sys_exit(0)


def run_console_launcher():
    """Contains all console launcher logic"""
    current_version: str = get_current_version()

    def parse_arguments() -> Namespace:
        parser = ArgumentParser(
            description='Educational local CVS',
            usage='edu_cvs.py [--version] [-d | --debug] [-h | --help] '
                  'command [args]',
            formatter_class=RawDescriptionHelpFormatter,
            epilog=_EDU_CVS_COMMANDS_HELP)

        parser.add_argument(
            '-d', '--debug',
            action='store_true',
            help='turn on debugging mode')

        parser.add_argument(
            '--version',
            action='version',
            version=current_version)

        parser.add_argument(
            'command',
            type=str,
            nargs='+')

        return parser.parse_args()

    def try_to_process_cvs_command(arguments: Namespace) -> str:
        """Tries to process cvs command.

        Returns CVS info message in case of success
        """
        try:
            return ControlVersionSystem(
                arguments.command).process_cvs_command()

        # If something is wrong with console input then it is important to
        # inform user about this
        except ConsoleInputError as occurred_exc:
            if occurred_exc.args[0] == 'Unknown command':
                exit_with_exception(
                    'Unknown command is given',
                    occurred_exc,
                    arguments.debug)

            if occurred_exc.args[0] == 'No input for "add"':
                exit_with_exception(
                    'Nothing specified, nothing added',
                    occurred_exc,
                    arguments.debug)

            if occurred_exc.args[0] == 'add: multiple files':
                exit_with_exception(
                    'Multiple files were given for "add" command',
                    occurred_exc,
                    arguments.debug)

            exit_with_exception(
                'Something is wrong with console input ',
                occurred_exc,
                arguments.debug)

        # It is important to inform user if OS error occurred
        except OSError as occurred_err:
            if occurred_err.args[0] == 'init: cannot create folder':
                exit_with_exception(
                    'Cannot create ".edu-cvs" folder during initialization',
                    occurred_err,
                    arguments.debug)

            exit_with_exception(
                'Cannot process data in/from file: ' + occurred_err.filename,
                occurred_err,
                arguments.debug)

        except ProgramDataFormatError as occurred_err:
            exit_with_exception(
                'Data in edu-cvs file is damaged. Repository data '
                'cannot be restored: ' + occurred_err.filename,
                occurred_err,
                arguments.debug)

        except CodecException as occurred_exc:
            if occurred_exc.args == ('add: init repo first',):
                exit_with_exception(
                    'Init repository first via "edu-cvs.py init"',
                    occurred_exc,
                    arguments.debug)

            exit_with_exception(
                'Something broke inside Edu-cvs',
                occurred_exc,
                arguments.debug)

        except ProgramException as occurred_exc:
            exit_with_exception(
                'Something broke inside Edu-cvs',
                occurred_exc,
                arguments.debug)

        except Exception as occurred_exc:
            exit_with_exception(
                'Some exception occurred while cvs command was processing',
                occurred_exc,
                arguments.debug)

        # This code executes if some exception processing was forgotten
        exit_with_exception(
            'Something broke inside Edu-cvs',
            ProgramException(
                "[try..except] block in launcher logic was passed"),
            arguments.debug)

    success_info_message: str = try_to_process_cvs_command(parse_arguments())

    print(success_info_message)


_EDU_CVS_COMMANDS_HELP: str = """
CVS commands:

start a working area
  init         create an empty repository

work on the current change
  add          add single file name to staged version
"""
