using LaboratoryWork2_Oleshko.Models;
using LaboratoryWork2_Oleshko.Repository.Implementation;
using LaboratoryWork2_Oleshko.Utils;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Windows.Forms;

namespace LaboratoryWork2_Oleshko.Forms
{
    public partial class TeacherForm : MaterialForm
    {
        private Teacher teacher;

        public TeacherForm()
        {
            InitializeComponent();
            MaterialSkinManager.Instance.AddFormToManage(this);
        }

        public TeacherForm(Teacher teacher) : this()
        {
            this.teacher = teacher;
            ShowAdditionalDataFormIfNeeded();
            ShowApplicants();
            ShowFailedApplicants();
            ShowPassedApplicants();
        }

        private void ShowAdditionalDataFormIfNeeded()
        {
            if (IsAdditionalDataNeeded(teacher))
            {
                var additionalDataForm = new WriteAdditionalDataForm(teacher);
                additionalDataForm.Show();
                additionalDataForm.FormClosed += (c, arg) => Show();
            }
            else
            {
                Show();
            }
        }

        private bool IsAdditionalDataNeeded(Teacher teacher)
        {
            return string.IsNullOrEmpty(teacher.Email) || string.IsNullOrEmpty(teacher.FirstName) ||
                   string.IsNullOrEmpty(teacher.LastName) || string.IsNullOrEmpty(teacher.PhoneNumber);
        }

        private void ShowApplicants()
        {
            applicantsDataGridView.Rows.Clear();
            foreach(var answer in Repository<Answer>.GetRepo(Filenames.ApplicantAnswerFileName).GetAll())
            {
                var applicant = Repository<Applicant>.GetRepo(Filenames.ApplicantsFileName).GetByFilter(a => a.Id == answer.ApplicantId);
                if(applicant.ApplicantStatus == Applicant.Status.AwaitingResponse)
                {
                    var exam = Repository<Exam>.GetRepo(Filenames.ExamsFileName).GetByFilter(e => e.Id == answer.ExamId);
                    var faculty = Repository<Faculty>.GetRepo(Filenames.FacultiesFileName).GetByFilter(f => f.Id == exam.FacultyId);
                    applicantsDataGridView.Rows.Add(applicant.Id, faculty.Id, exam.Id, applicant.FirstName, faculty.Name, exam.ExamName, faculty.ScoreToPass); ;
                }
            }
        }

        private void ShowFailedApplicants()
        {
            ShowApplicantsByStatus(failedDataGridView, Applicant.Status.FailedExams);
        }

        private void ShowPassedApplicants()
        {
            ShowApplicantsByStatus(passedDataGridView, Applicant.Status.PassedExams);
        }

        private void ShowApplicantsByStatus(DataGridView dataGridView, Applicant.Status status)
        {
            dataGridView.Rows.Clear();
            foreach (var answer in Repository<Answer>.GetRepo(Filenames.ApplicantAnswerFileName).GetAll())
            {
                var applicant = Repository<Applicant>.GetRepo(Filenames.ApplicantsFileName).GetByFilter(a => a.Id == answer.ApplicantId);
                if (applicant.ApplicantStatus == status)
                {
                    var exam = Repository<Exam>.GetRepo(Filenames.ExamsFileName).GetByFilter(e => e.Id == answer.ExamId);
                    var faculty = Repository<Faculty>.GetRepo(Filenames.FacultiesFileName).GetByFilter(f => f.Id == exam.FacultyId);
                    var grade = Repository<Grade>.GetRepo(Filenames.GradesFileName).GetByFilter(g => g.ApplicantId == applicant.Id);
                    dataGridView.Rows.Add(applicant.LastName, faculty.Name, exam.ExamName, grade.Score, faculty.ScoreToPass);
                }
            }
        }

        private void setMarksButton_Click(object sender, EventArgs e)
        {
            var applicantId = Guid.Parse(applicantsDataGridView.CurrentRow.Cells[0].Value.ToString());
            var applicant = Repository<Applicant>.GetRepo(Filenames.ApplicantsFileName).GetByFilter(a => a.Id == applicantId);
            var applicantAnswer = Repository<Answer>.GetRepo(Filenames.ApplicantAnswerFileName).GetByFilter(a => a.ApplicantId == applicant.Id);

            var form = new RateApplicantForm(applicantAnswer);
            Hide();
            form.Show();
            form.FormClosed += (s, arg) =>
            {
                ShowApplicants();
                ShowFailedApplicants();
                ShowPassedApplicants();
                Show();
            };
        }
    }
}
