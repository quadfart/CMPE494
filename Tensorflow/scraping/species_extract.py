import json
import pandas as pd
import re

# Load the JSON file with species data
with open("plantnet300K_species_id_2_name.json", "r", encoding="utf-8") as f:
    species_data = json.load(f)

# Convert the JSON data into a DataFrame
species_df = pd.DataFrame(list(species_data.items()), columns=["id", "name"])

# Load the Excel file with superclasses (using the first row as header)
superclasses_df = pd.read_excel("data.xlsx", header=0)  # header=0 indicates first row is the header

# Normalize the names by stripping leading/trailing spaces and converting to lowercase
species_df["name_normalized"] = species_df["name"].str.strip().str.lower()
superclasses_df["Scientific Names_normalized"] = superclasses_df["Scientific Names"].str.strip().str.lower()
names_list=superclasses_df["Scientific Names_normalized"]
species = []
genus = []
for name in names_list:
    if name.lower().endswith("spp."):
        genus.append(name.split()[0])
    else:
        species.append(name)
print("Genus list:", genus)
print("Number of genus entries:", len(genus))
print("Species list:", species)
print("Number of species entries:", len(species))


# Function to match genus from genus list
def match_genus(genus_list, species_df):
    matches = []
    for genus in genus_list:
        # Remove punctuation and compare genus
        genus_clean = re.sub(r'[^\w\s]', '', genus).strip().lower()

        # Filter rows where the first word of the name matches the genus
        matched_rows = species_df[species_df['name_normalized'].str.split().str[0] == genus_clean]

        for _, row in matched_rows.iterrows():
            matches.append((row['id'], row['name']))
    return matches


# Function to match species from species list
def match_species(species_list, species_df):
    matches = []
    for species_name in species_list:
        # Clean species name and split it into the first two words
        species_parts = re.sub(r'[^\w\s]', '', species_name).strip().lower().split()[:2]

        # Loop through the DataFrame to compare the first two words
        for _, row in species_df.iterrows():
            name_parts = row['name_normalized'].split()[:2]  # Get the first two words of the name
            if species_parts == name_parts:
                matches.append((row['id'], row['name']))

    return matches


# Find genus matches
genus_matches = match_genus(genus, species_df)
print("Genus matches:", genus_matches)
print(f'{len(genus_matches)} genus Match')

# Find species matches
species_matches = match_species(species, species_df)
print("Species matches:", species_matches)
print(f'{len(species_matches)} species Match')