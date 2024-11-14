import os
import requests
import uuid
import tkinter as tk
from tkinter import filedialog

def select_directory():
    root = tk.Tk()
    root.withdraw()
    directory = filedialog.askdirectory(title="Target Directory")
    return directory

save_directory = select_directory()

def download_images(num_images, save_dir = save_directory):
    if not os.path.exists(save_dir):
        os.makedirs(save_dir)

    for _ in range(num_images):
        url = "https://picsum.photos/500"

        response = requests.get(url)

        random_filename = f"{uuid.uuid4()}.jpg"

        file_path = os.path.join(save_directory, random_filename)

        if response.status_code == 200:
            with open(file_path, "wb") as f:
                f.write(response.content)
            print(f"Image saved as {file_path}")
        else:
            print(f"Failed to fetch image #{_ + 1}")

download_images(50)