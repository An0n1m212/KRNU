using System.Windows;
using Microsoft.Win32;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Скачивание PDF-сертификата (используется в профиле и в «Моих результатах»).</summary>
public static class CertificateHelper
{
    public static void Download(AppServices s, Result result, Window? owner)
    {
        var loc = s.Localization;
        if (result.Status == ResultStatus.PendingTeacherReview || !result.IsPassed)
        {
            MessageBox.Show(loc["Msg_NoCertificate"]); return;
        }

        var student = s.Users.GetById(result.StudentId);
        var test = s.Tests.GetById(result.TestId);

        var dialog = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = $"certificate_{result.Id}.pdf" };
        if (dialog.ShowDialog(owner) != true) return;

        try
        {
            s.Export.ExportCertificatePdf(new CertificateData(
                student?.FullName ?? "", test?.Title ?? "—", result.Percentage, result.Date, $"TS-{result.Id:D6}"), dialog.FileName);
            MessageBox.Show(loc["Msg_Saved"]);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{loc["Err_ExportFailed"]}\n{ex.Message}");
        }
    }
}
