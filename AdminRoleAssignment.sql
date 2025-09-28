-- Bu SQL script'ini veritabanınızda çalıştırarak mevcut kullanıcınızı Admin yapabilirsiniz

-- 1. Önce kullanıcınızın ID'sini bulun
SELECT Id, UserName, Email FROM AspNetUsers WHERE Email = 'your-email@example.com';

-- 2. Admin rolünün ID'sini bulun
SELECT Id, Name FROM AspNetRoles WHERE Name = 'Admin';

-- 3. Kullanıcınızı Admin rolüne atayın (ID'leri yukarıdaki sorgulardan alın)
INSERT INTO AspNetUserRoles (UserId, RoleId) 
VALUES ('YOUR_USER_ID', 'ADMIN_ROLE_ID');

-- Örnek kullanım:
-- INSERT INTO AspNetUserRoles (UserId, RoleId) 
-- VALUES ('12345678-1234-1234-1234-123456789012', '87654321-4321-4321-4321-210987654321');
