import 'dart:convert';
import 'package:http/http.dart' as http;

class SensorService {
  final String baseUrl;

  SensorService(this.baseUrl);

  Future<List<dynamic>> fetchSensorDataByUserId(String userId) async {
    final response = await http.get(Uri.parse('$baseUrl/api/SensorData/GetSensorDataByUserId/user/$userId'));

    if (response.statusCode == 200) {
      final jsonResponse = jsonDecode(response.body);
      return jsonResponse['data'] ?? []; // Extract the 'data' field from the response
    } else {
      throw Exception('Failed to load sensor data');
    }
  }

  Future<void> addSensorData({required String sensorSerialNumber, required String userId}) async {
    final response = await http.post(
      Uri.parse('$baseUrl/api/SensorData/AddSensorData'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        "temperature": 0,
        "moisture": 0,
        "userId": int.parse(userId),
        "sensorSerialNumber": sensorSerialNumber,
      }),
    );

    if (response.statusCode != 200) {
      throw Exception('Failed to add sensor data');
    }
  }
}
