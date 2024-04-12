using LaboratoryWork2_Oleshko.Models;
using LaboratoryWork2_Oleshko.Repository.Implementation;
using LaboratoryWork2_Oleshko.Utils;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Windows.Forms;

namespace LaboratoryWork2_Oleshko.Forms
{
    public partial class RateApplicantForm : MaterialForm
    {
        private Answer applicantAnswer;
        public RateApplicantForm()
        {
            InitializeComponent();
            MaterialSkinManager.Instance.AddFormToManage(this);
        }

        public RateApplicantForm(Answer answer) : this()
        {
            applicantAnswer = answer;
            var exam = Repository<Exam>.GetRepo(Filenames.ExamsFileName).GetByFilter(e => e.Id == applicantAnswer.ExamId);

            applicantAnswerLabel.Text = "Applicant answer: " + GetAnswerText(applicantAnswer.AnswerIndex, exam);
            correctAnswerLabel.Text = "Correct answer: " + GetAnswerText(exam.IndexOfCorrectAnswer, exam);
            questionLabel.Text = "Question: " + exam.Question;
            SetFormTitle(answer.ApplicantId);
        }

        private string GetAnswerText(int answerIndex, Exam exam)
        {
            return exam.Answers[answerIndex];
        }

        private void SetFormTitle(Guid applicantId)
        {
            string applicantLastName = Repository<Applicant>.GetRepo(Filenames.ApplicantsFileName).GetByFilter(a => a.Id == applicantId).LastName;
            Text = $"Rate: {applicantLastName}";
        }

        private void rateButton_Click(object sender, EventArgs e)
        {
            var grade = new Grade
            {
                ApplicantId = applicantAnswer.ApplicantId,
                ExamId = applicantAnswer.ExamId,
                Score = int.Parse(rateTextField.Text)
            };

            UpdateApplicantStatus(grade);
            Repository<Grade>.GetRepo(Filenames.GradesFileName).Create(grade);

            MessageBox.Show("Applicant Rated.");
            Close();
        }

        private void UpdateApplicantStatus(Grade grade)
        {
            var applicant = Repository<Applicant>.GetRepo(Filenames.ApplicantsFileName)
                .GetByFilter(a => a.Id == applicantAnswer.ApplicantId);
            var exam = Repository<Exam>.GetRepo(Filenames.ExamsFileName)
                .GetByFilter(e => e.Id == applicantAnswer.ExamId);
            var faculty = Repository<Faculty>.GetRepo(Filenames.FacultiesFileName)
                .GetByFilter(f => f.Id == exam.FacultyId);

            applicant.ApplicantStatus = grade.Score < faculty.ScoreToPass ?
                Applicant.Status.FailedExams : Applicant.Status.PassedExams;
        }

        private void rateTextField_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }
    }
}
