using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

class Student
{
    public string Name { get; set; }
    public string RollNumber { get; set; }
    public string RegistrationNumber { get; set; }
    public double Sem1SGPA { get; set; }
    public double Sem2SGPA { get; set; }
    public double Sem3SGPA { get; set; }
    public double AverageSGPA => (Sem1SGPA + Sem2SGPA + Sem3SGPA) / 3;
}

class Program
{
    static string connStr = "server=localhost;user=root;password=raunak;database=studentdb;";

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n=== Student SGPA Management ===");
            Console.WriteLine("1. Add New Student");
            Console.WriteLine("2. View All Students");
            Console.WriteLine("3. Delete Student");
            Console.WriteLine("4. Save Data to File");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddStudents();
                    break;
                case "2":
                    ViewStudents();
                    break;
                case "3":
                    DeleteStudent();
                    break;
                case "4":
                    SaveDataToFile();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    static void AddStudents()
    {
        Console.Write("Enter number of students to add: ");
        int n = int.Parse(Console.ReadLine());
        List<Student> students = new List<Student>();

        for (int i = 0; i < n; i++)
        {
            Console.WriteLine($"\n--- Enter details for student {i + 1} ---");
            Student student = new Student();

            Console.Write("Name: ");
            student.Name = Console.ReadLine();

            Console.Write("Roll Number: ");
            student.RollNumber = Console.ReadLine();

            Console.Write("Registration Number: ");
            student.RegistrationNumber = Console.ReadLine();

            Console.Write("1st Sem SGPA: ");
            student.Sem1SGPA = double.Parse(Console.ReadLine());

            Console.Write("2nd Sem SGPA: ");
            student.Sem2SGPA = double.Parse(Console.ReadLine());

            Console.Write("3rd Sem SGPA: ");
            student.Sem3SGPA = double.Parse(Console.ReadLine());

            students.Add(student);
        }

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();

            foreach (var student in students)
            {
                string query = @"INSERT INTO students 
                (name, roll_number, registration_number, sem1_sgpa, sem2_sgpa, sem3_sgpa, avg_sgpa)
                VALUES (@name, @roll, @reg, @sem1, @sem2, @sem3, @avg);";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", student.Name);
                cmd.Parameters.AddWithValue("@roll", student.RollNumber);
                cmd.Parameters.AddWithValue("@reg", student.RegistrationNumber);
                cmd.Parameters.AddWithValue("@sem1", student.Sem1SGPA);
                cmd.Parameters.AddWithValue("@sem2", student.Sem2SGPA);
                cmd.Parameters.AddWithValue("@sem3", student.Sem3SGPA);
                cmd.Parameters.AddWithValue("@avg", student.AverageSGPA);

                cmd.ExecuteNonQuery();
            }

            conn.Close();
        }

        Console.WriteLine("\nStudents added successfully!");
    }




    static void SaveDataToFile()
    {
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "students.txt");


        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();

            string query = "SELECT name, roll_number, registration_number, sem1_sgpa, sem2_sgpa, sem3_sgpa, avg_sgpa FROM students ORDER BY avg_sgpa DESC;";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Name\t\tRoll No\tReg No\t\tSem1\tSem2\tSem3\tAvg SGPA");

                while (reader.Read())
                {
                    string line = $"{reader["name"]}\t{reader["roll_number"]}\t{reader["registration_number"]}\t" +
                                  $"{reader["sem1_sgpa"]}\t{reader["sem2_sgpa"]}\t{reader["sem3_sgpa"]}\t" +
                                  $"{Convert.ToDouble(reader["avg_sgpa"]):F2}";

                    writer.WriteLine(line);
                }
            }

            conn.Close();
        }

        Console.WriteLine($"\n✅ Data saved successfully to '{filePath}'!");
    }




    static void ViewStudents()
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();

            string query = "SELECT name, roll_number, registration_number, sem1_sgpa, sem2_sgpa, sem3_sgpa, avg_sgpa FROM students ORDER BY avg_sgpa DESC;";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\n--- Stored Student Records ---");
            Console.WriteLine("Name\tRoll No\tReg No\tSem1\tSem2\tSem3\tAvg SGPA");

            while (reader.Read())
            {
                Console.WriteLine($"{reader["name"]}\t{reader["roll_number"]}\t{reader["registration_number"]}\t" +
                                  $"{reader["sem1_sgpa"]}\t{reader["sem2_sgpa"]}\t{reader["sem3_sgpa"]}\t" +
                                  $"{Convert.ToDouble(reader["avg_sgpa"]):F2}");
            }

            conn.Close();
        }
    }

    static void DeleteStudent()
    {
        Console.Write("\nEnter Roll Number of student to delete: ");
        string rollNo = Console.ReadLine();

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();

            string query = "DELETE FROM students WHERE roll_number = @roll;";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@roll", rollNo);

            int affectedRows = cmd.ExecuteNonQuery();

            if (affectedRows > 0)
                Console.WriteLine("Student deleted successfully.");
            else
                Console.WriteLine("No student found with that Roll Number.");

            conn.Close();
        }
    }
}


