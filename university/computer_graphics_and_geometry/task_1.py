from typing import Optional, Dict, List, Tuple
from tkinter import (
    Tk,
    Canvas,
    PhotoImage,
    Label,
    Entry,
    Toplevel,
    Button,
    mainloop,
    NSEW)


class FuncDrawer:
    _VARS_NAMES: List[str] = ['y_abs_max', 'alpha', 'beta', 'a', 'b', 'c']

    _AXES_INDENT: int = 0
    _COORDINATE_GRID_INDENT: int = 40
    _DASH_HALF_DIAMETER: int = 5

    _GREY_COLOR: str = "#aaaaaa"
    _RED_COLOR: str = "#FF0000"
    _BLUE_COLOR: str = '#0000FF'

    _root: Tk
    _canvas: Canvas
    _canvas_image: PhotoImage

    _setup_window: Toplevel
    _func_image: PhotoImage
    _func_image_label: Label
    _input_labels: Dict[str, Label]
    _input_entries: Dict[str, Entry]
    _draw_button: Button

    _current_vars_values: Dict[str, float]

    _canvas_center: Tuple[int, int]
    _x_in_pixel: float
    _y_in_pixel: float

    def __init__(self):
        self._root = Tk()

        self._current_vars_values = {name: 0 for name in self._VARS_NAMES}

    def init_tkinter(self):
        self._root.minsize(100, 100)

        self._init_setup_window()

        self._init_canvas()

        # self._root.bind(
        #     "<Configure>", lambda event: self._redraw_func(event))

    def _init_canvas(self):
        self._canvas = Canvas(
            self._root,
            width=640,
            height=480,
            bd=-2)

        self._root.grid_columnconfigure(1, weight=1)
        self._root.grid_rowconfigure(0, weight=1)

        self._canvas.grid(column=1, sticky=NSEW)

    def _init_setup_window(self):
        self._setup_window = Toplevel(master=self._root)

        self._setup_window.resizable(False, False)

        self._func_image = PhotoImage(file="task_1_func.png")
        self._func_image_label = Label(
            master=self._setup_window, image=self._func_image)

        # This fixes image disappearing
        self._func_image_label.image = self._func_image

        self._input_entries = dict()
        self._input_labels = dict()
        for name in self._VARS_NAMES:
            self._input_labels[name] = (
                Label(master=self._setup_window, text="Ввод " + name))
            self._input_entries[name] = Entry(master=self._setup_window)

            self._input_entries[name].insert(0, "1")

        self._draw_button = Button(
            master=self._setup_window,
            text="Построить график",
            command=self._draw_new_func)

        self._func_image_label.grid(row=0)
        for i, name in enumerate(self._VARS_NAMES):
            self._input_labels[name].grid(row=2 * i + 1)
            self._input_entries[name].grid(row=2 * i + 2)
        self._draw_button.grid(row=len(self._VARS_NAMES) * 2 + 1)

    # def _redraw_func(self, event):
    #     """Draws func if new width or height was given"""
    #     if (event.width != self._canvas_width
    #             or event.height != self._canvas_height):
    #         self._draw_func()

    def _draw_new_func(self):
        """Changes consts and invokes [_draw_func] if consts were changed"""
        consts_were_changed: bool = self._change_consts()

        if consts_were_changed:
            self._draw_func()
        else:
            print("Something is wrong with some entry value")

    def _change_consts(self) -> bool:
        """Changes consts if correct ones were given

        Returns [True] if consts were changed and [False] otherwise
        """
        try:
            for name in self._VARS_NAMES:
                float(self._input_entries[name].get())
        except ValueError:
            return False

        for name in self._VARS_NAMES:
            self._current_vars_values[name] = (
                float(self._input_entries[name].get()))

        return True

    def _draw_func(self):
        self._create_canvas_image()

        self._draw_coordinate_plane()

        self._draw_func_pixels()

    def _create_canvas_image(self):
        self._canvas.delete('all')

        self._canvas_image = PhotoImage(
            width=self._canvas.winfo_width(),
            height=self._canvas.winfo_height())

        # Magical fix of PhotoImage garbage collection
        self._canvas.image = self._canvas_image

        self._canvas.create_image(
            (int(self._canvas.winfo_width()) / 2,
             int(self._canvas.winfo_height()) / 2),
            image=self._canvas.image,
            state="normal")

    def _draw_coordinate_plane(self):
        self._canvas_center = (
            self._canvas.winfo_width() // 2, self._canvas.winfo_height() // 2)

        self._draw_horizontal_lines()
        self._draw_horizontal_axis_arrow()

        self._draw_vertical_lines()

        self._calculate_scaling()

        # self._draw_x_scale_division()
        # self._draw_y_scale_division()

    def _draw_horizontal_lines(self):
        self._draw_horizontal_line(
            self._AXES_INDENT,
            self._canvas.winfo_width() - self._AXES_INDENT,
            self._canvas_center[1],
            self._RED_COLOR)

        for i in range(
                1, self._canvas_center[1] // self._COORDINATE_GRID_INDENT + 1):
            self._draw_horizontal_line(
                self._AXES_INDENT,
                self._canvas.winfo_width() - self._AXES_INDENT,
                self._canvas_center[1] + i * self._COORDINATE_GRID_INDENT)
            self._draw_horizontal_line(
                self._AXES_INDENT,
                self._canvas.winfo_width() - self._AXES_INDENT,
                self._canvas_center[1] - i * self._COORDINATE_GRID_INDENT)

    def _draw_horizontal_axis_arrow(self):
        for i in range(1, 10):
            self._canvas_image.put(
                self._RED_COLOR,
                (self._canvas.winfo_width() - self._AXES_INDENT - i,
                 self._canvas_center[1] - i // 2))
            self._canvas_image.put(
                self._RED_COLOR,
                (self._canvas.winfo_width() - self._AXES_INDENT - i,
                 self._canvas_center[1] + i // 2))

    def _draw_vertical_lines(self):
        for i in range(
                1,
                self._canvas.winfo_width()
                // self._COORDINATE_GRID_INDENT + 1):
            self._draw_vertical_line(
                self._AXES_INDENT,
                self._canvas.winfo_height() - self._AXES_INDENT,
                i * self._COORDINATE_GRID_INDENT)

        # # Draws pseudo OY axis for ordinates scale division drawing further
        # self._draw_vertical_line(
        #     self._AXES_INDENT,
        #     self._canvas.winfo_height() - self._AXES_INDENT,
        #     2 * self._COORDINATE_GRID_INDENT,
        #     self._RED_COLOR)

    def _calculate_scaling(self):
        self._x_in_pixel = (
                abs(self._current_vars_values['beta']
                    - self._current_vars_values['alpha'])
                / self._canvas.winfo_width())

        self._y_in_pixel = (
            self._current_vars_values['y_abs_max'] * 2
            / self._canvas.winfo_height())

    # def _draw_x_scale_division(self):
    #     # Draw vertical dash
    #     self._draw_vertical_line(
    #         self._canvas_center[1] - self._DASH_HALF_DIAMETER,
    #         self._canvas_center[1] + self._DASH_HALF_DIAMETER,
    #         self._COORDINATE_GRID_INDENT,
    #         self._RED_COLOR)
    #
    #     # Draw number in front of the dash
    #     self._canvas.create_text(
    #         (self._COORDINATE_GRID_INDENT,
    #          self._canvas_center[1] + self._DASH_HALF_DIAMETER + 10),
    #         text='{:.2f}'.format(float(self._COORDINATE_GRID_INDENT)
    #                              * self._x_in_pixel
    #                              + self._current_vars_values['alpha']),
    #         fill='red')
    #
    # def _draw_y_scale_division(self):
    #     # Draw horizontal dash
    #     self._draw_horizontal_line(
    #         2 * self._COORDINATE_GRID_INDENT - self._DASH_HALF_DIAMETER,
    #         2 * self._COORDINATE_GRID_INDENT + self._DASH_HALF_DIAMETER,
    #         self._canvas_center[1] - self._COORDINATE_GRID_INDENT,
    #         self._RED_COLOR)
    #     pass

    def _draw_horizontal_line(
            self, x1: int, x2: int, y: int, color: str = _GREY_COLOR):
        for _i in range(x1, x2 + 1):
            self._canvas_image.put(color, (_i, y))

    def _draw_vertical_line(
            self, y1: int, y2: int, x: int, color: str = _GREY_COLOR):
        for _i in range(y1, y2 + 1):
            self._canvas_image.put(color, (x, _i))

    def _draw_func_pixels(self):
        for xx in range(self._canvas.winfo_width()):
            y: Optional[float] = self.get_func_value(
                     self._current_vars_values['alpha']
                     + xx * self._x_in_pixel,
                     self._current_vars_values['a'],
                     self._current_vars_values['b'],
                     self._current_vars_values['c'])

            if y is not None:
                yy: int = int(abs(
                    (y / self._y_in_pixel) - self._canvas_center[1]))

                if 0 <= yy < self._canvas.winfo_height():
                    self._canvas_image.put(self._BLUE_COLOR, (xx, yy))

    @staticmethod
    def get_func_value(
            _x: float,
            _a: float,
            _b: float,
            _c: float) -> Optional[float]:
        if _b + _x == 0 or _c - _x == 0:
            return None
        else:
            return (_a * _x) / ((_b + _x) * pow(_c - _x, 2))


if __name__ == "__main__":
    FuncDrawer().init_tkinter()

    mainloop()
