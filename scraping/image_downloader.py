import os
import requests
import pandas as pd
from tqdm import tqdm
import tkinter as tk
from tkinter import filedialog
from concurrent.futures import ThreadPoolExecutor, as_completed
import psutil


def select_top_directory():
    root = tk.Tk()
    root.withdraw()
    directory = filedialog.askdirectory(title="Select Top-Level Directory with Subfolders")
    return directory


def download_image(url, save_path, max_size=8 * 1024 * 1024):
    try:
        head_response = requests.head(url, allow_redirects=True)
        content_length = int(head_response.headers.get('Content-Length', 0))

        if content_length > max_size:
            print(f"Skipping {url} - File size is greater than {max_size / (1024 * 1024)} MB")
            return

        response = requests.get(url, stream=True)
        content_type = response.headers.get('Content-Type')

        if response.status_code == 200 and content_type == 'image/jpeg':
            with open(save_path, 'wb') as f:
                for chunk in response.iter_content(1024):
                    f.write(chunk)
        else:
            print(f"Skipping {url} - Content type is not image/jpeg or request failed")
    except requests.exceptions.RequestException as e:
        print(f"Error downloading {url} - {e}")


def process_folder(folder_path):
    print(f"\nProcessing folder: {folder_path}")

    # Check for multimedia.txt and inaturalist.csv
    multimedia_txt_path = os.path.join(folder_path, 'multimedia.txt')
    inaturalist_csv_path = os.path.join(folder_path, 'inaturalist.csv')

    save_directory = os.path.join(folder_path, "images")
    os.makedirs(save_directory, exist_ok=True)

    images_to_download = []

    # Process multimedia.txt if it exists
    if os.path.exists(multimedia_txt_path):
        print("Found multimedia.txt, processing...")
        df = pd.read_csv(multimedia_txt_path, sep='\t')

        image_column = 'identifier'
        format_column = 'format'

        for index, row in df.iterrows():
            if row[format_column] == "image/jpeg":
                image_url = row[image_column]
                file_extension = '.jpeg' if image_url.endswith('.jpeg') else '.jpg'
                file_name = os.path.join(save_directory, f'image_{index}{file_extension}')

                if not os.path.exists(file_name):
                    images_to_download.append((image_url, file_name))

    # If multimedia.txt doesn't exist, check for inaturalist.csv
    elif os.path.exists(inaturalist_csv_path):
        print("Found inaturalist.csv, processing...")
        df = pd.read_csv(inaturalist_csv_path)

        # Assuming the first column contains the URLs
        for index, row in df.iterrows():
            image_url = row[0]  # First column has the image URL
            file_extension = '.jpeg' if image_url.endswith('.jpeg') else '.jpg'
            file_name = os.path.join(save_directory, f'image_{index}{file_extension}')

            if not os.path.exists(file_name):
                images_to_download.append((image_url, file_name))

    else:
        print(f"No multimedia.txt or inaturalist.csv found in {folder_path}, skipping.")
        return

    if not images_to_download:
        print(f"All images are already downloaded for {folder_path}, skipping.")
        return

    # Proceed with downloading the images using ThreadPoolExecutor
    cpu_count = psutil.cpu_count(logical=True)
    max_threads = cpu_count

    print(f"Using {max_threads} threads for downloading images in {folder_path}")

    with ThreadPoolExecutor(max_workers=max_threads) as executor:
        futures = []
        for image_url, file_name in images_to_download:
            futures.append(executor.submit(download_image, image_url, file_name))

        for future in tqdm(as_completed(futures), desc=f"Downloading images for {folder_path}", total=len(futures)):
            future.result()


top_directory = select_top_directory()
print(f"Selected top-level directory: {top_directory}")

# Get all folder paths that contain either multimedia.txt or inaturalist.csv
folder_paths = [os.path.join(top_directory, folder_name) for folder_name in os.listdir(top_directory)
                if os.path.isdir(os.path.join(top_directory, folder_name)) and
                ('multimedia.txt' in os.listdir(os.path.join(top_directory, folder_name)) or
                 'inaturalist.csv' in os.listdir(os.path.join(top_directory, folder_name)))]

with ThreadPoolExecutor() as folder_executor:
    folder_futures = {folder_executor.submit(process_folder, folder_path): folder_path for folder_path in folder_paths}

    for future in tqdm(as_completed(folder_futures), desc="Processing all folders", total=len(folder_futures)):
        future.result()

print("All downloads complete.")
