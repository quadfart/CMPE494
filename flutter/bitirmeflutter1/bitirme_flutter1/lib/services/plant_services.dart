import 'dart:convert';
import 'package:http/http.dart' as http;

class PlantServices {
  final String baseUrl;

  PlantServices(this.baseUrl);

  Future<Map<String, dynamic>> fetchPlantDataById(
      {required String plantId}) async {
    final response =
        await http.get(Uri.parse('$baseUrl/api/Plant/PlantById/$plantId'));

    if (response.statusCode == 200) {
      final jsonResponse = jsonDecode(response.body);
      return jsonResponse['data'] ??
          []; // Extract the 'data' field from the response
    } else {
      throw Exception('Failed to load plant data');
    }
  }

  Future<dynamic> findDisease(String imagePath) async {
    final url = Uri.parse(
        'http://ec2-13-53-214-45.eu-north-1.compute.amazonaws.com:80/api/SensorData/DiseasePrediction');

    try {
      // Prepare multipart request
      final request = http.MultipartRequest('POST', url);

      // Add the image file
      request.files.add(await http.MultipartFile.fromPath('file', imagePath));

      // Send the request
      final response = await request.send();

      if (response.statusCode == 200) {
        // Parse response
        final responseData = await http.Response.fromStream(response);

        final data = jsonDecode(responseData.body);

        final dataBody = data['data'];

        final highestConfidenceDisease = dataBody.reduce((a, b) {
          return a['diseasePredictionConfidence'] >
                  b['diseasePredictionConfidence']
              ? a
              : b;
        });

        return highestConfidenceDisease;
      } else {
        throw Exception(
            'Failed to predict plant. Status code: ${response.statusCode}');
      }
    } catch (e) {
      throw Exception('Error predicting plant: $e');
    }
  }

  Future<List<dynamic>> predictPlant(String imagePath) async {
    final url = Uri.parse(
        'http://ec2-13-53-214-45.eu-north-1.compute.amazonaws.com:80/api/Plant/PlantPrediction');

    try {
      // Prepare multipart request
      final request = http.MultipartRequest('POST', url);

      // Add the image file
      request.files.add(await http.MultipartFile.fromPath('file', imagePath));

      // Send the request
      final response = await request.send();

      if (response.statusCode == 200) {
        // Parse response
        final responseData = await http.Response.fromStream(response);
        print('Response: ${responseData.body}');

        final data = jsonDecode(responseData.body);
        print('Data: $data');
        final dataList = data['data'] as List;
        return dataList;
      } else {
        throw Exception(
            'Failed to predict plant. Status code: ${response.statusCode}');
      }
    } catch (e) {
      throw Exception('Error predicting plant: $e');
    }
  }

  Future<void> addSensorData(
      {required String sensorSerialNumber, required String userId}) async {
    final response = await http.post(
      Uri.parse('$baseUrl/api/SensorData/AddSensorData'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        "userId": int.parse(userId),
        "sensorSerialNumber": sensorSerialNumber,
      }),
    );

    if (response.statusCode != 200) {
      throw Exception('Failed to add sensor data');
    }
  }

  Future<void> updateSensorData({
    required String plantId,
    required int id,
    int diseaseId = 1,
  }) async {
    final response = await http.put(
      Uri.parse('$baseUrl/api/SensorData/UpdateSensorData'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        "id": id,
        "plantId": int.parse(plantId),
        "diseaseId": diseaseId,
      }),
    );

    if (response.statusCode != 200) {
      throw Exception('Failed to update sensor data');
    }
  }
}
