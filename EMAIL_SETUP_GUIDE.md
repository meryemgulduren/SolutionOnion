# 📧 Email Gönderimi Kurulum Rehberi

## Gmail SMTP Ayarları

### 1. Gmail'de 2 Adımlı Doğrulama Aktifleştirin
1. Gmail hesabınıza giriş yapın
2. **Google Hesabı** > **Güvenlik** bölümüne gidin
3. **2 Adımlı Doğrulama**'yı açın

### 2. App Password (Uygulama Şifresi) Oluşturun
1. **Google Hesabı** > **Güvenlik** > **2 Adımlı Doğrulama**
2. **Uygulama şifreleri**'ne tıklayın
3. **Uygulama seç** > **Diğer (Özel ad)** > "Proposal System" yazın
4. **Oluştur** butonuna tıklayın
5. **16 haneli şifreyi** kopyalayın (örn: `abcd efgh ijkl mnop`)

### 3. appsettings.json Dosyasını Güncelleyin

```json
{
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "From": "your-email@gmail.com",
    "UserName": "your-email@gmail.com",
    "Password": "your-16-digit-app-password"
  }
}
```

### 4. Örnek Ayarlar
```json
{
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "From": "meryem@gmail.com",
    "UserName": "meryem@gmail.com",
    "Password": "abcd efgh ijkl mnop"
  }
}
```

## Diğer Email Sağlayıcıları

### Outlook/Hotmail
```json
{
  "EmailSettings": {
    "Host": "smtp-mail.outlook.com",
    "Port": 587,
    "EnableSsl": true,
    "From": "your-email@outlook.com",
    "UserName": "your-email@outlook.com",
    "Password": "your-password"
  }
}
```

### Yandex
```json
{
  "EmailSettings": {
    "Host": "smtp.yandex.com",
    "Port": 587,
    "EnableSsl": true,
    "From": "your-email@yandex.com",
    "UserName": "your-email@yandex.com",
    "Password": "your-password"
  }
}
```

## Test Etme

1. Projeyi çalıştırın
2. Yeni bir hesap oluşturun
3. Email'inizi kontrol edin
4. Doğrulama linkine tıklayın

## Güvenlik Notları

- ✅ App Password kullanın (normal şifre değil)
- ✅ 2 Adımlı Doğrulama aktif olsun
- ✅ Güvenli bağlantı (SSL/TLS) kullanın
- ❌ Normal Gmail şifresi kullanmayın
- ❌ Ayarları public repository'de paylaşmayın

## Hata Durumları

### "Authentication failed" Hatası
- App Password doğru mu?
- 2 Adımlı Doğrulama aktif mi?

### "Connection timeout" Hatası
- İnternet bağlantısı var mı?
- Firewall engelliyor mu?

### "Invalid credentials" Hatası
- Email adresi doğru mu?
- App Password doğru kopyalandı mı?
