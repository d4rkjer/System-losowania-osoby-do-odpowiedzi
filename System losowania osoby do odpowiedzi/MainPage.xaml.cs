using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Maui.Controls;

namespace System_losowania_osoby_do_odpowiedzi;

public partial class MainPage : ContentPage
{
    private Dictionary<string, List<string>> studentsDict = new Dictionary<string, List<string>>();
    private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "students632.txt");
    private Random random = new Random();
    private int luckyNumber;

    public MainPage()
    {
        InitializeComponent();
        classPicker.SelectedIndexChanged += ClassPicker_SelectedIndexChanged;

        if (File.Exists(filePath))
        {
            LoadStudentsFromFile();
        }
    }

    private async void LoadStudentsFromFile()
    {
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] parts = line.Split(':');
                    string className = parts[0];
                    List<string> students = parts[1].Split(',').ToList();
                    studentsDict.Add(className, students);

                    classPicker.ItemsSource = studentsDict.Keys.ToList();
                    classEntry.Text = "";
                }
            }

            // Update class picker
            classPicker.ItemsSource = studentsDict.Keys.ToList();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load students: {ex.Message}", "OK");
        }
    }

    private void ClassPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string selectedClass = classPicker.SelectedItem?.ToString();
        if (!string.IsNullOrWhiteSpace(selectedClass) && studentsDict.ContainsKey(selectedClass))
        {
            luckyNumber = random.Next(studentsDict[selectedClass].Count);
            StringBuilder studentsListBuilder = new StringBuilder();
            studentsListBuilder.AppendLine("Students in Class:");

            studentsListBuilder.AppendLine($"Lucky number: {luckyNumber}");
            foreach (string student in studentsDict[selectedClass])
            {
                studentsListBuilder.AppendLine(student);
            }

            studentsListLabel.Text = studentsListBuilder.ToString();
        }
    }

    private async void SaveStudentsToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var entry in studentsDict)
                {
                    await writer.WriteLineAsync($"{entry.Key}:{string.Join(",", entry.Value)}");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save students: {ex.Message}", "OK");
        }
    }

    private async void AddClassButton_Clicked(object sender, EventArgs e)
    {
        string newClass = classEntry.Text.Trim();
        if (!string.IsNullOrWhiteSpace(newClass) && !studentsDict.ContainsKey(newClass))
        {
            studentsDict.Add(newClass, new List<string>());

            Device.BeginInvokeOnMainThread(() =>
            {
                classPicker.ItemsSource = studentsDict.Keys.ToList();
                classEntry.Text = "";
            });
        }
    }

    private void AddStudentButton_Clicked(object sender, EventArgs e)
    {
        string selectedClass = classPicker.SelectedItem?.ToString();
        string newStudent = studentEntry.Text.Trim();
        if (!string.IsNullOrWhiteSpace(selectedClass) && !string.IsNullOrWhiteSpace(newStudent))
        {
            studentsDict[selectedClass].Add(newStudent);
            studentEntry.Text = "";

            if (!string.IsNullOrWhiteSpace(selectedClass) && studentsDict.ContainsKey(selectedClass))
            {
                StringBuilder studentsListBuilder = new StringBuilder();
                studentsListBuilder.AppendLine("Students in Class:");
                studentsListBuilder.AppendLine($"Lucky number: {luckyNumber}");

                foreach (string student in studentsDict[selectedClass])
                {
                    studentsListBuilder.AppendLine(student);
                }

                studentsListLabel.Text = studentsListBuilder.ToString();
            }
        }
    }

    // Metoda do losowania ucznia z uwzględnieniem szczęśliwego numerka
    private async void PickStudentButton_Clicked(object sender, EventArgs e)
    {
        string selectedClass = classPicker.SelectedItem?.ToString();
        if (!string.IsNullOrWhiteSpace(selectedClass) && studentsDict.ContainsKey(selectedClass) && studentsDict[selectedClass].Any())
        {
            int numberOfStudents = studentsDict[selectedClass].Count;

            string selectedStudent = studentsDict[selectedClass][random.Next(numberOfStudents)];
            int randomed = random.Next(numberOfStudents);
            if (randomed == luckyNumber)
            {
                await DisplayAlert("Picked Student", "Lucky number!", "OK");
            } else
            {
                await DisplayAlert("Picked Student", selectedStudent, "OK");
            }
            await DisplayAlert("Picked Student", selectedStudent, "OK");
        }
        else
        {
            await DisplayAlert("Error", "No students available in selected class", "OK");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        SaveStudentsToFile();
    }
}