from unittest import main as unittest_main, TestCase
from os import (
    pardir as os_pardir,
    getcwd as os_getcwd)
from os.path import (
    join as os_path_join,
    dirname as os_path_dirname,
    abspath as os_path_abspath
)
from sys import path as sys_path
from shutil import rmtree as shutil_rmtree

sys_path.append(os_path_join(
    os_path_dirname(os_path_abspath(__file__)),
    os_pardir))

from cvs.cvs_main import ControlVersionSystem, ConsoleInputError


class CVSCommandsProcessing(TestCase):
    def test_unknown_command(self):
        cvs: ControlVersionSystem = ControlVersionSystem(['123'])

        with self.assertRaises(ConsoleInputError) as exc_handle:
            cvs.process_cvs_command()

        self.assertEqual(('Unknown command', ), exc_handle.exception.args)

    def test_init(self):
        cvs: ControlVersionSystem = ControlVersionSystem(['init', '123'])

        self.assertEqual(
            "Initialized Edu-cvs repository in " + os_getcwd(),
            cvs.process_cvs_command())

        shutil_rmtree(cvs._changes_codec._CVS_DIR_PATH)

    def test_add_no_files(self):
        cvs: ControlVersionSystem = ControlVersionSystem(['add'])

        with self.assertRaises(ConsoleInputError) as exc_handle:
            cvs.process_cvs_command()

        self.assertEqual(('No input for "add"',), exc_handle.exception.args)

    def test_add_multiple_files(self):
        cvs: ControlVersionSystem = ControlVersionSystem(['add', '123', '321'])

        with self.assertRaises(ConsoleInputError) as exc_handle:
            cvs.process_cvs_command()

        self.assertEqual(
            ('add: multiple files',), exc_handle.exception.args)

    def test_add_success(self):
        cvs: ControlVersionSystem = ControlVersionSystem(['init'])

        cvs.process_cvs_command()

        temp_path: str = os_path_join(cvs._changes_codec._CVS_DIR_PATH, '123')

        open(temp_path, 'w').close()

        cvs = ControlVersionSystem(['add', temp_path])

        self.assertEqual('success: ' + temp_path, cvs.process_cvs_command())

        shutil_rmtree(cvs._changes_codec._CVS_DIR_PATH)


if __name__ == '__main__':
    unittest_main()
