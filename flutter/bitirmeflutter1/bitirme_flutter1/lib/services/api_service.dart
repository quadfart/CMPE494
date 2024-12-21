import 'dart:convert';
import 'package:http/http.dart' as http;

class ApiService {
  static const String baseUrl = "http://192.168.31.100:5273/api/Auth";

  // Login API call
  static Future<Map<String, dynamic>?> login(
      String email, String password) async {
    final url = Uri.parse("$baseUrl/Login");  // Correct endpoint

    try {
      final response = await http.post(
        url,
        headers: {"Content-Type": "application/json"},
        body: jsonEncode({
          "email": email,
          "password": password,
        }),
      );

      print("Response body: ${response.body}");  // Debugging response

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);

        // Check if 'data' is returned in the response
        if (data.containsKey('data')) {
          return data['data'];  // Return user data
        } else {
          print("Login failed: No user data in response");
          return null;
        }
      } else {
        print("Login Failed: ${response.statusCode}");
        return null;
      }
    } catch (e) {
      print("Connection Error: $e");
      return null;
    }
  }
}
