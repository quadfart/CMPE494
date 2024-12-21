import 'package:bitirme_flutter1/login.dart';
import 'package:flutter/material.dart';
import 'services/sign_up_service.dart'; // Import the SignUpService
import 'package:shared_preferences/shared_preferences.dart';


class SignUpScreen extends StatefulWidget {
  const SignUpScreen({super.key});

  @override
  _SignUpScreenState createState() => _SignUpScreenState();
}

class _SignUpScreenState extends State<SignUpScreen> {
  final TextEditingController _usernameController = TextEditingController();
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _sensorSerialNumberController = TextEditingController();

  bool _isLoading = false;
  String? _errorMessage;

  // Sign-up function to call the service
  _signUp() async {
    String username = _usernameController.text;
    String email = _emailController.text;
    String password = _passwordController.text;

    // Show loading indicator while processing
    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    // Call the SignUpService
    final signUpService = SignUpService();
    final response = await signUpService.signUp(username, email, password);

    // Hide the loading indicator
    setState(() {
      _isLoading = false;
    });

    if (response['success']) {
      // Store user login status in SharedPreferences
      SharedPreferences prefs = await SharedPreferences.getInstance();
      prefs.setString('user_token', 'new_user_logged_in');

      // Navigate to the homepage
      Navigator.pushReplacement(
        context,
        MaterialPageRoute(builder: (context) => LoginScreen()),
      );
    } else {
      // Display error message if sign-up fails
      setState(() {
        _errorMessage = response['message'];
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Sign Up')),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            TextField(
              controller: _usernameController,
              decoration: const InputDecoration(labelText: 'Username'),
            ),
            TextField(
              controller: _emailController,
              decoration: const InputDecoration(labelText: 'Email'),
            ),
            TextField(
              controller: _passwordController,
              decoration: const InputDecoration(labelText: 'Password'),
              obscureText: true,
            ),
            TextField(
              controller: _sensorSerialNumberController,
              decoration: const InputDecoration(labelText: 'Sensor Serial Number'),
            ),
            const SizedBox(height: 20),
            _isLoading
                ? const CircularProgressIndicator()
                : ElevatedButton(
              onPressed: _signUp,
              style: ElevatedButton.styleFrom(backgroundColor: Colors.blue),
              child: const Text('Sign Up'),
            ),
            if (_errorMessage != null) ...[
              const SizedBox(height: 10),
              Text(
                _errorMessage!,
                style: const TextStyle(color: Colors.red),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
