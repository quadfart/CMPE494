import 'dart:convert';
import 'package:http/http.dart' as http;

class ModelService {
  final String baseUrl;

  ModelService({required this.baseUrl});

  // Function to predict plant details using POST request
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
}
