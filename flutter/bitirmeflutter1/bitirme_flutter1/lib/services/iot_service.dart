import 'dart:convert';
import 'package:http/http.dart' as http;

class IotService {
  final String baseUrl;

  IotService(this.baseUrl);

  Future<List<dynamic>> fetchIotDataById(
      {required String id}) async {
    final response = await http
        .get(Uri.parse('$baseUrl/api/SensorData/SensorDataLogById/$id'));

    if (response.statusCode == 200) {
      final jsonResponse = jsonDecode(response.body);
      final data = jsonResponse['data'];
      
      return data ?? []; // Extract the 'data' field from the response
    } else {
      throw Exception('Failed to load iot data');
    }
  }
}
