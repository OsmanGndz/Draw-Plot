import tkinter as tk
from tkinter import filedialog, messagebox
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
import matplotlib.pyplot as plt
import socket
import threading

plt.rcParams.update({'font.size': 12})

class Plot:
    def __init__(self, color):
        self.color = color
    
    def set_color(self, color):
        self.color = color
    
    def get_color(self):
        return self.color
    
    def draw(self, plt):
        pass

class Line(Plot):
    def __init__(self, color, x_data, y_data, title, line_width, line_style, xlabel, ylabel):
        super().__init__(color)
        self.x_data = x_data
        self.y_data = y_data
        self.title = title
        self.line_width = line_width
        self.line_style = line_style
        self.xlabel = xlabel
        self.ylabel = ylabel

    def draw(self, plt):
        plt.plot(self.x_data, self.y_data, color=self.color, linewidth=self.line_width, linestyle=self.line_style)
        plt.title(self.title)
        plt.xlabel(self.xlabel)
        plt.ylabel(self.ylabel)

class Scatter(Plot):
    def __init__(self, color, x_data, y_data, title, point_size, marker_style, xlabel, ylabel):
        super().__init__(color)
        self.x_data = x_data
        self.y_data = y_data
        self.title = title
        self.point_size = point_size
        self.marker_style = marker_style
        self.xlabel = xlabel
        self.ylabel = ylabel

    def draw(self, plt):
        plt.scatter(self.x_data, self.y_data, color=self.color, s = self.point_size, marker = self.marker_style)
        plt.title(self.title)
        plt.xlabel(self.xlabel)
        plt.ylabel(self.ylabel)

class Bar(Plot):
    def __init__(self, color, x_data, y_data, title, bar_width, bar_hatch, xlabel, ylabel):
        super().__init__(color)
        self.x_data = x_data
        self.y_data = y_data
        self.title = title
        self.bar_width = bar_width
        self.bar_hatch = bar_hatch
        self.xlabel = xlabel
        self.ylabel = ylabel
    
    def draw(self, plt):
        plt.bar(self.x_data, self.y_data, color=self.color, width = self.bar_width, hatch=self.bar_hatch)
        plt.title(self.title)
        plt.xlabel(self.xlabel)
        plt.ylabel(self.ylabel)

class Histogram(Plot):
    def __init__(self, color, data, title, bin, density, cumulative, xlabel, ylabel):
        super().__init__(color)
        self.data = data
        self.title = title
        self.bin = bin
        self.density = density
        self.cumulative = cumulative
        self.xlabel = xlabel
        self.ylabel = ylabel
    
    def draw(self, plt):
        plt.hist(self.data, color=self.color, bins=self.bin, density = self.density, cumulative= self.cumulative)
        plt.title(self.title)
        plt.xlabel(self.xlabel)
        plt.ylabel(self.ylabel)

class Box(Plot):
    def __init__(self, color, data, title, symbolStyle, vertical, boxWidth, xlabel, ylabel):
        super().__init__(color)
        self.data = data
        self.title = title
        self.symbolStyle = symbolStyle
        self.vertical = vertical
        self.boxWidth = boxWidth
        self.xlabel = xlabel
        self.ylabel = ylabel
    
    def draw(self, plt):
        plt.boxplot(self.data, boxprops=dict(color =  self.color), sym=self.symbolStyle, vert = self.vertical, widths= self.boxWidth)
        plt.title(self.title)
        plt.xlabel(self.xlabel)
        plt.ylabel(self.ylabel)

class PlotApp(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("GeoShape Plot Application")
        self.geometry("800x600")

        # Create a figure for matplotlib
        self.figure, self.ax = plt.subplots(figsize=(5, 5), dpi=100)
        
        # Add a canvas to the Tkinter window
        self.canvas = FigureCanvasTkAgg(self.figure, self)
        self.canvas.get_tk_widget().pack(fill=tk.BOTH, expand=True)

        # Create menu
        menubar = tk.Menu(self)
        server_menu = tk.Menu(menubar, tearoff=0)
        server_menu.add_command(label="Start Server", command=self.start_server)
        menubar.add_cascade(label="Server", menu=server_menu)
        self.config(menu=menubar)

        self.shapes = []

    def clear_canvas(self):
        self.ax.clear()
        self.shapes.clear()

    def draw_shapes(self):
        self.ax.clear()
        for shape in self.shapes:
            shape.draw(plt)
        self.canvas.draw()

    def start_server(self):
        self.server_thread = threading.Thread(target=self.run_server, daemon=True)
        self.server_thread.start()
        self.ax.text(
            0.95, 0.01,
            "Server Started on port 8000",
            verticalalignment='bottom', horizontalalignment='right',
            transform=self.ax.transAxes,
            color='green', fontsize=15
        )
        self.canvas.draw()

    def run_server(self):
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.bind(('localhost', 8000))
        self.server_socket.listen(10)
        while True:
            client_socket, addr = self.server_socket.accept()
            client_thread = threading.Thread(target=self.handle_client, args=(client_socket,), daemon=True)
            client_thread.start()

    def handle_client(self, client_socket):
        with client_socket:
            while True:
                data = client_socket.recv(4096).decode()
                if not data:
                    break
                self.process_client_data(data)

    def process_client_data(self, datas):
        print(datas)
        lines = datas.strip().split('\n')
        x_data = []
        y_data = []
        color = None
        shape_type = None
        title = None
        par1 = None
        par2 = None
        par3 = None
        xlabel = None
        ylabel = None
        bin = None
        first_line = True
        
        for line in lines:
            parts = line.strip().split(",")

            if first_line and len(parts) < 9:
                print(f"Error: Not enough parts in line: {line}")
                continue
            elif not first_line and len(parts) < 1:
                print(f"Error: Not enough parts in line: {line}")
                continue

            try:
                # İlk satır için tüm bilgileri alın
                if first_line:
                    shape_type = parts[0]
                    color = parts[1]

                    if shape_type == "Line":
                        x_data.append(float(parts[2]))
                        y_data.append(float(parts[3]))
                        title = parts[4]
                        par1 = float(parts[5])
                        par2 = parts[6]
                        xlabel = parts[7]
                        ylabel = parts[8]

                    elif shape_type == "Scatter":
                        x_data.append(float(parts[2]))
                        y_data.append(float(parts[3]))
                        title = parts[4]
                        par1 = float(parts[5])
                        par2 = parts[6]
                        xlabel = parts[7]
                        ylabel = parts[8]

                    elif shape_type == "Bar":
                        x_data.append(float(parts[2]))
                        y_data.append(float(parts[3]))
                        title = parts[4]
                        par1 = float(parts[5])
                        par2 = parts[6]
                        xlabel = parts[7]
                        ylabel = parts[8]    

                    elif shape_type == "Histogram":
                        x_data.append(float(parts[2]))
                        title = parts[3]
                        bin = int(parts[4])
                        par1 = parts[5] == "True"
                        par2 = parts[6] == "True"
                        xlabel = parts[7]
                        ylabel = parts[8]

                    elif shape_type == "Box":
                        x_data.append(float(parts[2]))
                        title = parts[3]
                        par1 = parts[4]
                        par2 = parts[5] == "True"
                        par3 = float(parts[6])
                        xlabel = parts[7]
                        ylabel = parts[8]


                    first_line = False
                else:
                    if shape_type == "Histogram" or shape_type == "Box":
                        x_data.append(float(parts[0]))
                    else:
                        x_data.append(float(parts[0]))
                        y_data.append(float(parts[1]))     
                        
            except ValueError as e:
                print(f"Error processing line: {line}, Error: {e}")
                continue
            except IndexError as e:
                print(f"Error processing line: {line}, Error: {e}")
                continue

        # Clear existing shapes before adding the new one
        self.clear_canvas()
        # Şekli oluştur ve ekle
        if shape_type == "Line":
            shape = Line(color, x_data, y_data, title, par1, par2, xlabel, ylabel)
        elif shape_type == "Scatter":
            shape = Scatter(color, x_data, y_data, title, par1, par2, xlabel, ylabel)
        elif shape_type == "Bar":
            shape = Bar(color, x_data, y_data, title, par1, par2, xlabel, ylabel)
        elif shape_type == "Histogram":
            shape = Histogram(color, x_data, title, bin, par1, par2, xlabel, ylabel)
        elif shape_type == "Box":
            shape = Box(color, x_data, title, par1, par2, par3, xlabel, ylabel)
        else:
            print(f"Error: Unknown shape type {shape_type}")
            return

        self.shapes.append(shape)
        self.draw_shapes()
            

if __name__ == "__main__":
    app = PlotApp()
    app.mainloop()
