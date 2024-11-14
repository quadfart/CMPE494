import requests
import tkinter as tk
from tkinter import filedialog
from time import sleep, time


def select_directory():
    root = tk.Tk()
    root.withdraw()  # Hide the main tkinter window
    directory = filedialog.askdirectory(title="Select Download Directory")
    return directory


def download_file(url, destination, chunk_size=16 * 1024 * 1024, max_retries=3):
    retries = 0
    try:
        with requests.get(url, stream=True) as response:
            response.raise_for_status()
            total_size = int(response.headers.get("content-length", 0))
            with open(destination, "wb") as file:
                downloaded_size = 0
                for chunk in response.iter_content(chunk_size=chunk_size):
                    if chunk:
                        start_time = time()  # Track start time for the chunk
                        file.write(chunk)
                        end_time = time()  # Track end time for the chunk

                        # Update downloaded size and calculate progress
                        downloaded_size += len(chunk)
                        progress = downloaded_size / total_size * 100

                        # Calculate download speed
                        duration = end_time - start_time
                        speed = len(chunk) / duration / (1024 * 1024)  # Convert to MB/s

                        # Display progress and speed
                        print(f"\rProgress: {progress:.2f}% | Speed: {speed:.2f} MB/s", end="")

        print("\nDownload completed successfully.")
    except requests.exceptions.RequestException as e:
        if retries < max_retries:
            retries += 1
            print(f"\nRetrying ({retries}/{max_retries}) due to: {e}")
            sleep(2)
            download_file(url, destination, chunk_size, max_retries - retries)
        else:
            print("\nFailed to download after multiple retries.")


# Prompt user to select directory
download_dir = select_directory()
if download_dir:
    url = "https://zenodo.org/records/5645731/files/plantnet_300K.zip"
    file_name = "plantnet_300K.zip"
    destination = f"{download_dir}/{file_name}"
    download_file(url, destination)
else:
    print("No directory selected.")