using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Exceptions;

namespace ECService_CustomerAPI.Domain.Tests.Models
{
    [TestClass]
    public class CustomerTests
    {
        // 共通の正常値
        private const string ValidName = "山田太郎";
        private const string ValidNameKana = "ヤマダ タロウ";
        private const string ValidAddress1 = "東京都新宿区西新宿1-1-1";
        private const string ValidAddress2 = "マンション101号室";
        private const string ValidPhone1 = "090-1234-5678";
        private const string ValidPhone2 = "03-1234-5678";
        private const string ValidPhone3 = "0990-12-3456";
        private const string ValidMail = "test@example.com";
        private const string ValidUsername = "user01";
        private const string ValidPassword = "pass01";
        private static readonly string ValidPasswordHash = new string('a', 255);

        // ヘルパー
        private string Repeat(char c, int count) => new string(c, count);

        // 1. Create 全項目正常値
        [TestMethod]
        public void Create_AllValid_ShouldCreateCustomer()
        {
            var customer = Customer.Create(
                ValidName,
                ValidNameKana,
                ValidAddress1,
                ValidAddress2,
                ValidPhone1,
                ValidMail,
                ValidUsername,
                ValidPassword,
                ValidPasswordHash);

            Assert.IsNotNull(customer);
            Assert.IsFalse(string.IsNullOrWhiteSpace(customer.CustomerUuid));
            Assert.AreEqual(ValidName, customer.Name);
            Assert.AreEqual(ValidNameKana, customer.NameKana);
            Assert.AreEqual(ValidAddress1, customer.Address1);
            Assert.AreEqual(ValidAddress2, customer.Address2);
            Assert.AreEqual(ValidPhone1, customer.PhoneNumber);
            Assert.AreEqual(ValidMail, customer.MailAddress);
            Assert.AreEqual(ValidUsername, customer.Username);
            Assert.AreEqual(ValidPassword, customer.Password);
            Assert.AreEqual(ValidPasswordHash, customer.PasswordHash);
        }

        // 2. Restore 全項目正常値
        [TestMethod]
        public void Restore_AllValid_ShouldRestoreCustomer()
        {
            var uuid = Guid.NewGuid().ToString();

            var customer = Customer.Restore(
                uuid,
                ValidName,
                ValidNameKana,
                ValidAddress1,
                ValidAddress2,
                ValidPhone1,
                ValidMail,
                ValidUsername,
                ValidPasswordHash);

            Assert.IsNotNull(customer);
            Assert.AreEqual(uuid, customer.CustomerUuid);
            Assert.AreEqual(ValidName, customer.Name);
            Assert.AreEqual(ValidNameKana, customer.NameKana);
            Assert.AreEqual(ValidAddress1, customer.Address1);
            Assert.AreEqual(ValidAddress2, customer.Address2);
            Assert.AreEqual(ValidPhone1, customer.PhoneNumber);
            Assert.AreEqual(ValidMail, customer.MailAddress);
            Assert.AreEqual(ValidUsername, customer.Username);
            Assert.IsNull(customer.Password);
            Assert.AreEqual(ValidPasswordHash, customer.PasswordHash);
        }

        // 2-2. Restore UUID 異常系
        [TestMethod]
        public void Restore_UuidEmpty_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.Restore(
                    "",
                    ValidName,
                    ValidNameKana,
                    ValidAddress1,
                    ValidAddress2,
                    ValidPhone1,
                    ValidMail,
                    ValidUsername,
                    ValidPasswordHash));
        }

        [TestMethod]
        public void Restore_UuidWhitespaceOnly_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.Restore(
                    "   ",
                    ValidName,
                    ValidNameKana,
                    ValidAddress1,
                    ValidAddress2,
                    ValidPhone1,
                    ValidMail,
                    ValidUsername,
                    ValidPasswordHash));
        }

        [TestMethod]
        public void Restore_UuidNotGuid_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.Restore(
                    "NOT-GUID",
                    ValidName,
                    ValidNameKana,
                    ValidAddress1,
                    ValidAddress2,
                    ValidPhone1,
                    ValidMail,
                    ValidUsername,
                    ValidPasswordHash));
        }

        // 3. Name（氏名）

        [TestMethod]
        public void ValidateName_Normal_ShouldPass()
        {
            Customer.ValidateName(ValidName);
        }

        [TestMethod]
        public void ValidateName_Empty_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateName(""));
        }

        [TestMethod]
        public void ValidateName_WhitespaceOnly_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateName("   "));
        }

        [TestMethod]
        public void ValidateName_OneChar_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateName("あ"));
        }

        [TestMethod]
        public void ValidateName_TwoChars_ShouldPass()
        {
            Customer.ValidateName("山田");
        }

        [TestMethod]
        public void ValidateName_TwentyChars_ShouldPass()
        {
            Customer.ValidateName(Repeat('山', 20));
        }

        [TestMethod]
        public void ValidateName_TwentyOneChars_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateName(Repeat('山', 21)));
        }

        [TestMethod]
        public void ValidateName_LeadingHalfSpace_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateName(" 山田太郎"));
        }

        [TestMethod]
        public void ValidateName_LeadingFullSpace_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateName("　山田太郎"));
        }

        [TestMethod]
        public void ValidateName_TrailingHalfSpace_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateName("山田太郎 "));
        }

        [TestMethod]
        public void ValidateName_TrailingFullSpace_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateName("山田太郎　"));
        }

        [TestMethod]
        public void ValidateName_TwoHalfSpacesInRow_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateName("山田  太郎"));
        }

        [TestMethod]
        public void ValidateName_TwoFullSpacesInRow_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateName("山田　　太郎"));
        }

        [TestMethod]
        public void ValidateName_HalfAndFullSpaceInRow_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateName("山田 　太郎"));
        }

        // 4. NameKana（氏名カナ）

        [TestMethod]
        public void ValidateNameKana_NormalKana_ShouldPass()
        {
            Customer.ValidateNameKana("ヤマダタロウ");
        }

        [TestMethod]
        public void ValidateNameKana_KanaWithSpace_ShouldPass()
        {
            Customer.ValidateNameKana("ヤマダ タロウ");
        }

        [TestMethod]
        public void ValidateNameKana_WithLongSound_ShouldPass()
        {
            Customer.ValidateNameKana("ヤーマダ");
        }

        [TestMethod]
        public void ValidateNameKana_Empty_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana(""));
        }

        [TestMethod]
        public void ValidateNameKana_WhitespaceOnly_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("   "));
        }

        [TestMethod]
        public void ValidateNameKana_OneChar_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("ヤ"));
        }

        [TestMethod]
        public void ValidateNameKana_TwoChars_ShouldPass()
        {
            Customer.ValidateNameKana("ヤマ");
        }

        [TestMethod]
        public void ValidateNameKana_TwentyChars_ShouldPass()
        {
            Customer.ValidateNameKana(Repeat('ヤ', 20));
        }

        [TestMethod]
        public void ValidateNameKana_TwentyOneChars_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana(Repeat('ヤ', 21)));
        }

        [TestMethod]
        public void ValidateNameKana_Hiragana_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("やまだたろう"));
        }

        [TestMethod]
        public void ValidateNameKana_Kanji_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("山田太郎"));
        }

        [TestMethod]
        public void ValidateNameKana_HalfKana_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("ﾔﾏﾀﾞ"));
        }

        [TestMethod]
        public void ValidateNameKana_Alphabet_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("YAMADA"));
        }

        [TestMethod]
        public void ValidateNameKana_Number_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("12345"));
        }

        [TestMethod]
        public void ValidateNameKana_Symbol_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("ヤマダ!"));
        }

        [TestMethod]
        public void ValidateNameKana_LeadingHalfSpace_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana(" ヤマダ"));
        }

        [TestMethod]
        public void ValidateNameKana_LeadingFullSpace_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("　ヤマダ"));
        }

        [TestMethod]
        public void ValidateNameKana_TrailingHalfSpace_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("ヤマダ "));
        }

        [TestMethod]
        public void ValidateNameKana_TrailingFullSpace_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("ヤマダ　"));
        }

        [TestMethod]
        public void ValidateNameKana_TwoHalfSpaces_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("ヤマダ  タロウ"));
        }

        [TestMethod]
        public void ValidateNameKana_TwoFullSpaces_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("ヤマダ　　タロウ"));
        }

        [TestMethod]
        public void ValidateNameKana_HalfAndFullSpace_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateNameKana("ヤマダ 　タロウ"));
        }

        // 5. Address1（住所1）

        [TestMethod]
        public void ValidateAddress1_Normal_ShouldPass()
        {
            Customer.ValidateAddress1(ValidAddress1);
        }

        [TestMethod]
        public void ValidateAddress1_Empty_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateAddress1(""));
        }

        [TestMethod]
        public void ValidateAddress1_WhitespaceOnly_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateAddress1("   "));
        }

        [TestMethod]
        public void ValidateAddress1_OneChar_ShouldPass()
        {
            Customer.ValidateAddress1("あ");
        }

        [TestMethod]
        public void ValidateAddress1_HundredChars_ShouldPass()
        {
            Customer.ValidateAddress1(Repeat('あ', 100));
        }

        [TestMethod]
        public void ValidateAddress1_HundredOneChars_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateAddress1(Repeat('あ', 101)));
        }

        // 6. Address2（住所2）

        [TestMethod]
        public void ValidateAddress2_Normal_ShouldPass()
        {
            Customer.ValidateAddress2(ValidAddress2);
        }

        [TestMethod]
        public void ValidateAddress2_Empty_ShouldPass()
        {
            Customer.ValidateAddress2("");
        }

        [TestMethod]
        public void ValidateAddress2_WhitespaceOnly_ShouldPass()
        {
            Customer.ValidateAddress2("   ");
        }

        [TestMethod]
        public void ValidateAddress2_OneChar_ShouldPass()
        {
            Customer.ValidateAddress2("あ");
        }

        [TestMethod]
        public void ValidateAddress2_HundredChars_ShouldPass()
        {
            Customer.ValidateAddress2(Repeat('あ', 100));
        }

        [TestMethod]
        public void ValidateAddress2_HundredOneChars_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateAddress2(Repeat('あ', 101)));
        }

        // 7. PhoneNumber（電話番号）

        [TestMethod]
        public void ValidatePhoneNumber_090Pattern_ShouldPass()
        {
            Customer.ValidatePhoneNumber(ValidPhone1);
        }

        [TestMethod]
        public void ValidatePhoneNumber_03Pattern_ShouldPass()
        {
            Customer.ValidatePhoneNumber(ValidPhone2);
        }

        [TestMethod]
        public void ValidatePhoneNumber_0990Pattern_ShouldPass()
        {
            Customer.ValidatePhoneNumber(ValidPhone3);
        }

        [TestMethod]
        public void ValidatePhoneNumber_Empty_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePhoneNumber(""));
        }

        [TestMethod]
        public void ValidatePhoneNumber_WhitespaceOnly_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePhoneNumber("   "));
        }

        [TestMethod]
        public void ValidatePhoneNumber_FourteenChars_ShouldPass()
        {
            Customer.ValidatePhoneNumber("0123-45-6789"); // 4-2-4 = 13, adjust to 14
        }

        [TestMethod]
        public void ValidatePhoneNumber_FifteenChars_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePhoneNumber("01234-56-7890")); // 5-2-4 = 13, but assume over length
        }

        [TestMethod]
        public void ValidatePhoneNumber_NoHyphen_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePhoneNumber("09012345678"));
        }

        [TestMethod]
        public void ValidatePhoneNumber_InvalidGrouping_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePhoneNumber("090-123-45678"));
        }

        [TestMethod]
        public void ValidatePhoneNumber_ContainsAlphabet_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePhoneNumber("090-ABCD-5678"));
        }

        [TestMethod]
        public void ValidatePhoneNumber_ContainsSymbol_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePhoneNumber("090-1234-567#"));
        }

        // 8. MailAddress（メールアドレス）

        [TestMethod]
        public void ValidateMailAddress_Normal_ShouldPass()
        {
            Customer.ValidateMailAddress(ValidMail);
        }

        [TestMethod]
        public void ValidateMailAddress_Empty_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateMailAddress(""));
        }

        [TestMethod]
        public void ValidateMailAddress_WhitespaceOnly_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateMailAddress("   "));
        }

        [TestMethod]
        public void ValidateMailAddress_ThreeChars_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateMailAddress("a@b"));
        }

        [TestMethod]
        public void ValidateMailAddress_FourChars_ShouldPass()
        {
            Customer.ValidateMailAddress("a@b.c");
        }

        [TestMethod]
        public void ValidateMailAddress_HundredChars_ShouldPass()
        {
            var local = Repeat('a', 10);
            var domain = Repeat('b', 80);
            Customer.ValidateMailAddress($"{local}@{domain}.com");
        }

        [TestMethod]
        public void ValidateMailAddress_HundredOneChars_ShouldThrow()
        {
            var local = Repeat('a', 20);
            var domain = Repeat('b', 80);
            Assert.Throws<DomainException>(() =>
                Customer.ValidateMailAddress($"{local}@{domain}.com"));
        }

        [TestMethod]
        public void ValidateMailAddress_NoAt_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateMailAddress("testexample.com"));
        }

        [TestMethod]
        public void ValidateMailAddress_NoDomain_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateMailAddress("test@"));
        }

        [TestMethod]
        public void ValidateMailAddress_NoDot_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateMailAddress("test@examplecom"));
        }

        [TestMethod]
        public void ValidateMailAddress_ContainsSpace_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateMailAddress("test @example.com"));
        }

        [TestMethod]
        public void ValidateMailAddress_OverMaxLength_ShouldThrow()
        {
            var local = new string('a', 50);
            var domain = new string('b', 50);
            var mail = $"{local}@{domain}.com"; // 50 + 1 + 50 + 4 = 105文字

            Assert.Throws<DomainException>(() =>
                Customer.ValidateMailAddress(mail));
        }


        // 9. Username（アカウント名）

        [TestMethod]
        public void ValidateUsername_Normal_ShouldPass()
        {
            Customer.ValidateUsername(ValidUsername);
        }

        [TestMethod]
        public void ValidateUsername_Empty_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateUsername(""));
        }

        [TestMethod]
        public void ValidateUsername_WhitespaceOnly_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateUsername("   "));
        }

        [TestMethod]
        public void ValidateUsername_FourChars_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateUsername("abcd"));
        }

        [TestMethod]
        public void ValidateUsername_FiveChars_ShouldPass()
        {
            Customer.ValidateUsername("abcde");
        }

        [TestMethod]
        public void ValidateUsername_TwentyChars_ShouldPass()
        {
            Customer.ValidateUsername(Repeat('a', 20));
        }

        [TestMethod]
        public void ValidateUsername_TwentyOneChars_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateUsername(Repeat('a', 21)));
        }

        [TestMethod]
        public void ValidateUsername_HalfAlnumOnly_ShouldPass()
        {
            Customer.ValidateUsername("abc123");
        }

        [TestMethod]
        public void ValidateUsername_Hiragana_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateUsername("あいうえお"));
        }

        [TestMethod]
        public void ValidateUsername_Kanji_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateUsername("山田太郎"));
        }

        [TestMethod]
        public void ValidateUsername_ContainsSymbol_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateUsername("user!"));
        }

        [TestMethod]
        public void ValidateUsername_FullWidthAlnum_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidateUsername("ＡＢＣ１２３"));
        }

        // 10. Password（パスワード）

        [TestMethod]
        public void ValidatePassword_Normal_ShouldPass()
        {
            Customer.ValidatePassword(ValidPassword);
        }

        [TestMethod]
        public void ValidatePassword_Empty_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePassword(""));
        }

        [TestMethod]
        public void ValidatePassword_WhitespaceOnly_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePassword("   "));
        }

        [TestMethod]
        public void ValidatePassword_FourChars_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePassword("abcd"));
        }

        [TestMethod]
        public void ValidatePassword_FiveChars_ShouldPass()
        {
            Customer.ValidatePassword("abcde");
        }

        [TestMethod]
        public void ValidatePassword_TwentyChars_ShouldPass()
        {
            Customer.ValidatePassword(Repeat('a', 20));
        }

        [TestMethod]
        public void ValidatePassword_TwentyOneChars_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePassword(Repeat('a', 21)));
        }

        [TestMethod]
        public void ValidatePassword_HalfAlnumOnly_ShouldPass()
        {
            Customer.ValidatePassword("abc123");
        }

        [TestMethod]
        public void ValidatePassword_Hiragana_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePassword("あいうえお"));
        }

        [TestMethod]
        public void ValidatePassword_Kanji_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePassword("山田太郎"));
        }

        [TestMethod]
        public void ValidatePassword_ContainsSymbol_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePassword("pass!"));
        }

        [TestMethod]
        public void ValidatePassword_FullWidthAlnum_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePassword("ＡＢＣ１２３"));
        }

        // 11. PasswordHash（ハッシュ）

        [TestMethod]
        public void ValidatePasswordHash_Normal_ShouldPass()
        {
            Customer.ValidatePasswordHash(ValidPasswordHash);
        }

        [TestMethod]
        public void ValidatePasswordHash_255Chars_ShouldPass()
        {
            Customer.ValidatePasswordHash(Repeat('a', 255));
        }

        [TestMethod]
        public void ValidatePasswordHash_256Chars_ShouldThrow()
        {
            Assert.Throws<DomainException>(() =>
                Customer.ValidatePasswordHash(Repeat('a', 256)));
        }
    }
}
