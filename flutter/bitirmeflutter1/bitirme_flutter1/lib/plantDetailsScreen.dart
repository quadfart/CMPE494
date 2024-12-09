import 'package:flutter/material.dart';
import 'dart:io';

class PlantDetailsScreen extends StatelessWidget {
  final Map<String, String> plantData;

  const PlantDetailsScreen({Key? key, required this.plantData})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(plantData["plantName"] ?? "Plant Details"),
        backgroundColor: Colors.green,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.pop(context),
        ),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Fotoğraf
            if (plantData["photoPath"] != null)
              Container(
                height: 200,
                width: double.infinity,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(15),
                  image: DecorationImage(
                    image: FileImage(File(plantData["photoPath"]!)),
                    fit: BoxFit.cover,
                  ),
                ),
              ),
            const SizedBox(height: 20),

            // Bitki adı ve açıklama
            Text(
              "Plant Name: ${plantData["plantName"] ?? "Unknown"}",
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 10),
            Text(
              "Description: ${plantData["description"] ?? "No description provided."}",
              style: const TextStyle(
                fontSize: 16,
                color: Colors.grey,
              ),
            ),
            const SizedBox(height: 20),

            // IoT Verileri (Dummy Değerler)
            const Text(
              "IoT Data:",
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 10),
            Container(
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(15),
                color: Colors.grey[200],
              ),
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: const [
                  Text("Water Level: 60%", style: TextStyle(fontSize: 16)),
                  SizedBox(height: 10),
                  Text("Temperature: 22°C", style: TextStyle(fontSize: 16)),
                  SizedBox(height: 10),
                  Text("Humidity: 40%", style: TextStyle(fontSize: 16)),
                  SizedBox(height: 10),
                  Text("Soil Moisture: 30%", style: TextStyle(fontSize: 16)),
                ],
              ),
            ),

            const SizedBox(height: 30),

            // Geri Dön Butonu
            Center(
              child: ElevatedButton(
                onPressed: () {
                  Navigator.pop(context);
                },
                style: ElevatedButton.styleFrom(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 20,
                    vertical: 15,
                  ),
                  backgroundColor: Colors.green,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(10),
                  ),
                ),
                child: const Text(
                  "Back to My Plants",
                  style: TextStyle(fontSize: 16, color: Colors.white),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
