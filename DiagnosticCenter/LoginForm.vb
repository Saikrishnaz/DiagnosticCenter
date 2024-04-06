Imports System.Data.SqlClient
Imports Guna.UI2.WinForms
Imports System.Web.Configuration
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Public Class LoginForm
    Dim connectionString As String = "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\source\repos\DiagnosticCenter\DiagnosticCenter\DiagnosticCenterDatabase.mdf;Integrated Security=True"
    Dim con As New SqlConnection(connectionString)
    Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Guna2TabControl1.TabMenuVisible = False
        'closeformisopenandhide()
    End Sub

    Private Sub Guna2GradientButton3_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton3.Click
        Guna2TabControl1.SelectedTab = TabPage1
    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        Guna2TabControl1.SelectedTab = TabPage2
    End Sub
    Private Function AuthenticateUser() As Integer
        Dim level As Integer = 0
        ' Determine the column based on ComboBox selection
        If Guna2ComboBox1.SelectedIndex = 0 Then
            level = 1 ' Doctor
        ElseIf Guna2ComboBox1.SelectedIndex = 1 Then
            level = 2 ' Receptionist
        Else
            MsgBox("Please select the login type")
        End If

        Return level
    End Function






    Private Sub Guna2GradientButton5_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton5.Click
        Try
            Dim level As Integer = AuthenticateUser()
            If level > 0 Then
                Dim EmailID As String = Txt_Email.Text
                Dim password As String = Txt_Pass.Text

                Select Case level
                    Case 1 ' Doctor
                        Dim queryEmp As String = "SELECT StaffId FROM StaffData WHERE Desgnation='Doctor' AND Email = @Email AND Password = @Password"
                        Using cmd As New SqlCommand(queryEmp, con)
                            con.Open()
                            cmd.Parameters.AddWithValue("@Email", EmailID)
                            cmd.Parameters.AddWithValue("@Password", password)

                            ' Execute the query
                            Dim result As Object = cmd.ExecuteScalar()

                            If result IsNot Nothing Then
                                ' Login successful, retrieve the user ID
                                Dim loggedInUserID As Integer = Convert.ToInt32(result)

                                Dim Doctor As New DoctorForm()
                                MessageBox.Show("Login Successful. ID: " & loggedInUserID.ToString())
                                Doctor.TextBox1.Text = loggedInUserID.ToString()
                                Doctor.Show()
                                Me.Hide()
                                ' Clear textboxes upon successful login
                                Txt_Email.Clear()
                                Txt_Pass.Clear()
                            Else
                                ' Login failed
                                MsgBox("Invalid credentials. Please try again.")
                            End If
                        End Using

                    Case 2 ' Receptionist
                        Dim queryEmp As String = "SELECT StaffId FROM StaffData WHERE Desgnation='Receptionist' AND Email = @Email AND Password = @Password"
                        Using cmd As New SqlCommand(queryEmp, con)
                            con.Open()
                            cmd.Parameters.AddWithValue("@Email", EmailID)
                            cmd.Parameters.AddWithValue("@Password", password)

                            ' Execute the query
                            Dim result As Object = cmd.ExecuteScalar()

                            If result IsNot Nothing Then
                                ' Login successful, retrieve the user ID
                                Dim loggedInUserID As Integer = Convert.ToInt32(result)

                                Dim Reception As New Form1()
                                MessageBox.Show("Login Successful. ID: " & loggedInUserID.ToString())
                                Reception.TextBox1.Text = loggedInUserID.ToString()
                                Reception.Show()
                                Me.Hide()
                                ' Clear textboxes upon successful login
                                Txt_Email.Clear()
                                Txt_Pass.Clear()
                            Else
                                ' Login failed
                                MsgBox("Invalid credentials. Please try again.")
                            End If
                        End Using

                    Case Else ' default
                        MessageBox.Show("Please Select Login Role: Doctor / Receptionist")
                End Select

            Else
                MessageBox.Show("Please check your email and password.")
            End If

        Catch ex As Exception
            MessageBox.Show("Error occurred: " & ex.Message)

        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try



    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            Txt_Pass.PasswordChar = ControlChars.NullChar ' Display actual characters
        Else
            Txt_Pass.PasswordChar = "*" ' Display asterisks
        End If
    End Sub

    Private Function AuthenticateUser2() As Integer
        Dim level As Integer = 0

        ' Determine the column based on ComboBox selection
        If Guna2ComboBox2.SelectedIndex = 0 Then
            level = 1 ' Doctor
        ElseIf Guna2ComboBox1.SelectedIndex = 1 Then
            level = 2 ' Receptionist
        Else
            MsgBox("Please select the login type")
        End If

        Return level
    End Function

    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        Try
            Dim level As Integer = AuthenticateUser2()

            If level > 0 Then
                Dim email As String = Txt_ForGotEmail.Text
                Dim dob As Date = DTP_ForGotDOB.Value.Date
                Dim newPassword As String = Txt_ForGotPass1.Text
                Dim confirmPassword As String = Txt_ForGotPass2.Text

                Select Case level
                    Case 1 ' Doctor
                        ' Check if the new password and confirm password match
                        If newPassword = confirmPassword Then
                            ' Verify the user's email and date of birth
                            If VerifyStaff("StaffData", dob, email) Then
                                ' Update the password in the database
                                UpdatePassword("StaffData", email, newPassword)
                                MessageBox.Show("Password updated successfully.")
                            Else
                                MessageBox.Show("Invalid email or date of birth.")
                            End If
                        Else
                            MessageBox.Show("New password and confirm password do not match.")
                        End If
                    Case 2 ' Reveptionist
                        ' Check if the new password and confirm password match
                        If newPassword = confirmPassword Then
                            ' Verify the user's email and date of birth
                            If VerifyStaff("StaffData", dob, email) Then
                                ' Update the password in the database
                                UpdatePassword("StaffData", email, newPassword)
                                MessageBox.Show("Password updated successfully.")
                            Else
                                MessageBox.Show("Invalid email or date of birth.")
                            End If
                        Else
                            MessageBox.Show("New password and confirm password do not match.")
                        End If

                    Case Else ' default
                        MessageBox.Show("Please Select Login level: Employee / Customer")
                End Select
            Else
                MessageBox.Show("Please check your email and password.")
            End If

            Txt_ForGotEmail.Clear()
            Txt_ForGotPass1.Clear()
            Txt_ForGotPass2.Clear()
            Txt_ForGotPass2.Clear()
            DTP_ForGotDOB.Value = Date.Today
        Catch ex As Exception
            MessageBox.Show("Error occurred: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Function VerifyStaff(TblNAme As String, dob As Date, Email As String) As Boolean
        ' Implement the logic to verify the user's email and date of birth
        ' You should query your database to check if the provided email and DOB match a user's record
        ' Return True if the user is verified, otherwise return False
        Dim query As String = "SELECT COUNT(*) FROM " & TblNAme & " WHERE Email = @Email AND DOB = @DOB"
        Using cmd As New SqlCommand(query, con)
            cmd.Parameters.AddWithValue("@Email", Email)
            cmd.Parameters.AddWithValue("@DOB", dob)
            con.Open()
            Dim result As Integer = CInt(cmd.ExecuteScalar())
            con.Close()
            Return result > 0
        End Using
    End Function

    Private Sub UpdatePassword(tblname As String, email As String, newPassword As String)
        Dim query As String = "UPDATE " & tblname & " SET PassWord = @Password WHERE Email = @Email"
        Using cmd As New SqlCommand(query, con)
            cmd.Parameters.AddWithValue("@Email", email)
            cmd.Parameters.AddWithValue("@Password", newPassword)
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using
    End Sub
    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked = True Then
            Txt_ForGotPass2.PasswordChar = ControlChars.NullChar ' Display actual characters
            Txt_ForGotPass1.PasswordChar = ControlChars.NullChar ' Display actual characters
        Else
            Txt_ForGotPass2.PasswordChar = "*" ' Display asterisks
            Txt_ForGotPass1.PasswordChar = "*" ' Display asterisks
        End If

    End Sub

    Private Sub Guna2GradientPanel1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2GradientPanel1.Paint

    End Sub
End Class