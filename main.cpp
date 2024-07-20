#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <cstdlib>

std::vector<std::string> runStringsCommand(const std::string& filePath) {
    std::vector<std::string> stringsOutput;
    std::string command = "strings \"" + filePath + "\"";
    FILE* pipe = popen(command.c_str(), "r");
    if (!pipe) {
        std::cerr << "Error running strings command" << std::endl;
        return stringsOutput;
    }
    
    char buffer[128];
    while (fgets(buffer, sizeof(buffer), pipe) != nullptr) {
        stringsOutput.push_back(buffer);
    }

    pclose(pipe);
    return stringsOutput;
}

std::string detectLanguage(const std::vector<std::string>& stringsOutput) {
    for (const auto& str : stringsOutput) {
        if (str.find("MSVCRT") != std::string::npos) {
            return "C++ (Microsoft Visual C++)";
        } else if (str.find("libstdc++") != std::string::npos) {
            return "C++ (GNU GCC)";
        } else if (str.find("CPython") != std::string::npos) {
            return "Python";
        } else if (str.find("java") != std::string::npos) {
            return "Java";
        } else if (str.find("mono") != std::string::npos) {
            return "C# (Mono)";
        } else if (str.find("Go build") != std::string::npos) {
            return "Go";
        }
    }
    return "Unknown";
}

int main() {
    std::string filePath;
    std::cout << "Enter the path to the .exe file: ";
    std::getline(std::cin, filePath);

    std::vector<std::string> stringsOutput = runStringsCommand(filePath);
    std::string language = detectLanguage(stringsOutput);

    std::cout << "Detected language: " << language << std::endl;
    return 0;
}
