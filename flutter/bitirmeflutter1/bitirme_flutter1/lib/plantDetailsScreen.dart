import 'package:bitirme_flutter1/camera_screen.dart';
import 'package:bitirme_flutter1/services/iot_service.dart';
import 'package:bitirme_flutter1/services/plant_services.dart';
import 'package:bitirme_flutter1/services/sensor_services.dart';
import 'package:flutter/material.dart';
import 'dart:io';

import 'package:image_picker/image_picker.dart';

class PlantDetailsScreen extends StatefulWidget {
  final int plantId;
  final int id;
  final dynamic sensor;

  const PlantDetailsScreen({
    Key? key,
    required this.plantId,
    required this.id,
    required this.sensor,
  }) : super(key: key);

  @override
  State<PlantDetailsScreen> createState() => _PlantDetailsScreenState();
}

class _PlantDetailsScreenState extends State<PlantDetailsScreen> {
  final ImagePicker _picker = ImagePicker();
  late final PlantServices plantService;
  late final IotService iotService;
  late final SensorService sensorService;
  Map<String, dynamic> plantData = {};
  Map<String, dynamic> iotData = {};
  List<dynamic> iotList = [];
  bool isLoading = true;

  String diseaseName = "";
  List<dynamic> symptoms = [];
  List<dynamic> treatments = [];

  @override
  void initState() {
    plantService = PlantServices(
        'http://ec2-13-53-214-45.eu-north-1.compute.amazonaws.com:80');
    iotService = IotService(
        'http://ec2-13-53-214-45.eu-north-1.compute.amazonaws.com:80');
    sensorService = SensorService(
        'http://ec2-13-53-214-45.eu-north-1.compute.amazonaws.com:80');
    super.initState();
    fetchPlantData();
    print(widget.sensor);
  }

  void fetchPlantData() async {
    setState(() {
      isLoading = true;
    });
    plantData = widget.sensor["plant"];
    final disease = widget.sensor["disease"];
    if (disease != null) {
      diseaseName = disease["name"];
      symptoms = disease["symptoms"];
      treatments = disease["treatments"];
    }
    await fetchIotData();
    setState(() {
      isLoading = false;
    });
  }

  Future<void> fetchIotData() async {
    setState(() {
      isLoading = true;
    });
    iotList = await iotService.fetchIotDataById(id: widget.id.toString());
    iotData = iotList.last;
    setState(() {
      isLoading = false;
    });
  }

  Future<void> _saveDisease(int diseaseId) {
    return sensorService.updateSensorData(
      plantId: widget.plantId.toString(),
      id: widget.id,
      diseaseId: diseaseId,
    );
  }

  void _navigateToCameraScreen() async {
    final image = await Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => CameraScreen()),
    );
    if (image != null) {
      _findDisease(image);
    }
  }

  Future<void> _pickImage(ImageSource source) async {
    final pickedFile = await _picker.pickImage(source: source);
    if (pickedFile != null) {
      _findDisease(pickedFile);
    }
/*     if (pickedFile != null) {
      final imageBytes = await pickedFile.readAsBytes();
      setState(() {
        _selectedImage = _resizeImage(img.decodeImage(imageBytes)!, 224, 224);
      });
      if (_selectedImage != null) {
        processImage();
      }
    } */
  }

  Future<void> _findDisease(XFile image) async {
    final res = await plantService.findDisease(image.path);
    print(res);
    await _saveDisease(res["id"]);
    setState(() {
      diseaseName = res["name"];
      symptoms = res["symptoms"];
      treatments = res["treatments"];
    });
  }

  /*
  "modTemp": 22,
  "soilType": "Organic-rich, moisture-retaining but well-drained soil",
  "lightNeed": "Bright, indirect sunlight or low light",
  "humidityLevel": 70,
  "wateringFrequency": 1,
  "irrigationAmount": 1500,
  "scientificName": "philodendron hederaceum"
  */

  /**
   * id": 28,
    "temperature": 25,
    "moisture": 30,
    "timestamp": "2024-12-27T12:54:06.996421Z",
    "soilMoisture": 50
   */

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(plantData["scientificName"] ?? "Plant Details"),
        backgroundColor: Colors.green,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.pop(context),
        ),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: isLoading
            ? const Center(
                child: CircularProgressIndicator(
                  color: Colors.green,
                ),
              )
            : Column(
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
                    "Plant Name: ${plantData["scientificName"] ?? "Unknown"}",
                    style: const TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 10),
                  /* Text(
                    "Description: ${plantData["description"] ?? "No description provided."}",
                    style: const TextStyle(
                      fontSize: 16,
                      color: Colors.grey,
                    ),
                  ), */
                  ListView(
                    physics: const NeverScrollableScrollPhysics(),
                    shrinkWrap: true,
                    children: [
                      ListTile(
                        title: const Text("Temperature"),
                        subtitle: Text("${plantData["modTemp"]}°C"),
                      ),
                      ListTile(
                        title: const Text("Soil Type"),
                        subtitle: Text(plantData["soilType"]),
                      ),
                      ListTile(
                        title: const Text("Light Need"),
                        subtitle: Text(plantData["lightNeed"]),
                      ),
                      ListTile(
                        title: const Text("Humidity Level"),
                        subtitle: Text("${plantData["humidityLevel"]}%"),
                      ),
                      ListTile(
                        title: const Text("Watering Frequency"),
                        subtitle:
                            Text("${plantData["wateringFrequency"]} days"),
                      ),
                      ListTile(
                        title: const Text("Irrigation Amount"),
                        subtitle: Text("${plantData["irrigationAmount"]} ml"),
                      ),
                    ],
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
                      children: [
                        Text("Moisture: ${iotData["moisture"]}%",
                            style: TextStyle(fontSize: 16)),
                        SizedBox(height: 10),
                        Text("Temperature: ${iotData["temperature"]}°C",
                            style: TextStyle(fontSize: 16)),
                        SizedBox(height: 10),
                        Text("Time: ${iotData["timestamp"]}",
                            style: TextStyle(fontSize: 16)),
                        SizedBox(height: 10),
                        Text("Soil Moisture: ${iotData["soilMoisture"]}%",
                            style: TextStyle(fontSize: 16)),
                      ],
                    ),
                  ),

                  const SizedBox(height: 20),

                  // IoT Verileri (Dummy Değerler)
                  const Text(
                    "Diseases:",
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
                      children: [
                        Text("Name: $diseaseName",
                            style: const TextStyle(
                                fontSize: 16, fontWeight: FontWeight.bold)),
                        const SizedBox(height: 10),
                        const Text("Symptoms:",
                            style: TextStyle(
                                fontSize: 16, fontWeight: FontWeight.bold)),
                        ...symptoms.map((symptom) => Text(
                              "- $symptom",
                              style: const TextStyle(fontSize: 16),
                            )),
                        const SizedBox(height: 10),
                        const Text("Treatments:",
                            style: TextStyle(
                                fontSize: 16, fontWeight: FontWeight.bold)),
                        ...treatments.map((symptom) => Text(
                              "- $symptom",
                              style: const TextStyle(fontSize: 16),
                            )),
                        const SizedBox(height: 20),
                        const Text("Select image to identify disease:",
                            style: TextStyle(
                                fontSize: 16, fontWeight: FontWeight.bold)),
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                          children: [
                            ElevatedButton.icon(
                              onPressed: _navigateToCameraScreen,
                              icon: const Icon(Icons.camera_alt),
                              label: const Text('Take Photo'),
                              style: ElevatedButton.styleFrom(
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 16, vertical: 12),
                              ),
                            ),
                            ElevatedButton.icon(
                              onPressed: () => _pickImage(ImageSource.gallery),
                              icon: const Icon(Icons.photo_library),
                              label: const Text('Gallery'),
                              style: ElevatedButton.styleFrom(
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 16, vertical: 12),
                              ),
                            ),
                          ],
                        )
                      ],
                    ),
                  ),

                  const SizedBox(height: 30),
                  Center(
                    child: ElevatedButton.icon(
                      onPressed: () {
                        fetchIotData();
                      },
                      label: const Text("Update Sensor Data"),
                      icon: const Icon(Icons.update, color: Colors.white),
                      style: ElevatedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 20,
                          vertical: 15,
                        ),
                        backgroundColor: Colors.green,
                        foregroundColor: Colors.white,
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(10),
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(height: 20),
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
