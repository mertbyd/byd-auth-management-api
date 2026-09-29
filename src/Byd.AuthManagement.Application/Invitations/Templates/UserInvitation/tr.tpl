<!DOCTYPE html>
<html lang="tr">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Hesabınız hazır</title>
</head>
<body style="margin:0;padding:0;background-color:#f3f5f9;font-family:Segoe UI,Helvetica,Arial,sans-serif;color:#1f2937;">
  <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background-color:#f3f5f9;padding:32px 12px;">
    <tr>
      <td align="center">
        <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width:560px;background-color:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 2px 8px rgba(15,23,42,0.08);">
          <tr>
            <td style="background-color:#1d4ed8;padding:28px 32px;">
              <p style="margin:0;font-size:13px;letter-spacing:1px;text-transform:uppercase;color:#bfdbfe;">Davet</p>
              <h1 style="margin:8px 0 0;font-size:24px;line-height:32px;color:#ffffff;">Hesabınız hazır</h1>
            </td>
          </tr>
          <tr>
            <td style="padding:32px;">
              <p style="margin:0 0 16px;font-size:16px;line-height:24px;">Merhaba <strong>{{ model.name }}</strong>,</p>
              <p style="margin:0 0 16px;font-size:16px;line-height:24px;">Sizin için bir hesap oluşturuldu. Giriş yapabilmek için önce şifrenizi belirlemeniz gerekiyor.</p>
              <table role="presentation" cellspacing="0" cellpadding="0" style="margin:28px 0;">
                <tr>
                  <td style="border-radius:8px;background-color:#1d4ed8;">
                    <a href="{{ model.link }}" style="display:inline-block;padding:14px 28px;font-size:16px;font-weight:600;color:#ffffff;text-decoration:none;border-radius:8px;">Şifremi belirle</a>
                  </td>
                </tr>
              </table>
              <p style="margin:0 0 8px;font-size:14px;line-height:20px;color:#4b5563;">Buton çalışmazsa aşağıdaki bağlantıyı tarayıcınıza yapıştırın:</p>
              <p style="margin:0;font-size:13px;line-height:20px;word-break:break-all;"><a href="{{ model.link }}" style="color:#1d4ed8;">{{ model.link }}</a></p>
            </td>
          </tr>
          <tr>
            <td style="padding:20px 32px;background-color:#f9fafb;border-top:1px solid #e5e7eb;">
              <p style="margin:0;font-size:12px;line-height:18px;color:#6b7280;">Bu daveti beklemiyorsanız e-postayı yok sayabilirsiniz; şifre belirlenmeden hesaba giriş yapılamaz.</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
