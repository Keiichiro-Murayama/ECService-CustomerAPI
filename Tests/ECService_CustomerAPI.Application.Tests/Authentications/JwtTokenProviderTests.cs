using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ECService_CustomerAPI.Application.Authentications;
using ECService_CustomerAPI.Domain.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Application.Tests.Authentications;

[TestClass]
public class JwtTokenProviderTests
{
    /// <summary>
    /// トークンを生成でき、主要プロパティと署名が妥当であること
    /// </summary>
    [TestMethod(DisplayName = "トークンを生成でき、主要プロパティと署名が妥当であること")]
    public async Task IssueAccessToken_ReturnsToken_WithClaimIssuerAudienceExpiresAndSigningCredentials()
    {
        // Arrange
        var customer = CreateCustomer();
        var settings = new JwtSettings
        {
            Issuer = "ecservice-test-issuer",
            Audience = "ecservice-test-audience",
            SecretKey = "test-secret-key-for-jwt-signature-validation-12345",
            ExpiresInMinutes = 30,
        };
        var provider = new JwtTokenProvider(settings);
        var handler = new JsonWebTokenHandler();

        // Act
        var token = provider.IssueAccessToken(customer);

        // Assert: トークンが発行される
        Assert.IsFalse(string.IsNullOrWhiteSpace(token));

        // Assert: Claim / Issuer / Audience / Expires が含まれる
        var jwt = handler.ReadJsonWebToken(token);
        Assert.AreEqual(customer.CustomerUuid.ToString(), jwt.Subject);
        Assert.AreEqual(settings.Issuer, jwt.Issuer);
        Assert.AreEqual(settings.Audience, jwt.Audiences.Single());
        Assert.IsTrue(jwt.ValidTo > DateTime.UtcNow);

        // Assert: SigningCredentials が正しく設定され、署名検証できる
        Assert.AreEqual(SecurityAlgorithms.HmacSha256, jwt.Alg);
        Assert.IsFalse(string.IsNullOrWhiteSpace(jwt.EncodedSignature));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = settings.Issuer,
            ValidateAudience = true,
            ValidAudience = settings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
            ValidateLifetime = true,
            // 生成直後の時刻境界誤差で「not yet valid」になるのを防ぐ
            ClockSkew = TimeSpan.FromSeconds(5),
        };

        var validationResult = await handler.ValidateTokenAsync(token, validationParameters);
        Assert.IsTrue(validationResult.IsValid, validationResult.Exception?.Message);
    }

    /// <summary>
    /// 追加クレームが反映され、同名クレームは上書きされること
    /// </summary>
    [TestMethod(DisplayName = "追加クレームが反映され、同名クレームは上書きされること")]
    public void IssueAccessToken_ReflectsExtraClaims_AndOverridesSameClaimType()
    {
        // Arrange
        var customer = CreateCustomer();
        var settings = new JwtSettings
        {
            Issuer = "ecservice-test-issuer",
            Audience = "ecservice-test-audience",
            SecretKey = "test-secret-key-for-jwt-signature-validation-12345",
            ExpiresInMinutes = 30,
        };
        var provider = new JwtTokenProvider(settings);
        var handler = new JsonWebTokenHandler();
        var extraClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "override-subject"),
            new("role", "admin"),
        };

        // Act
        var token = provider.IssueAccessToken(customer, extraClaims);

        // Assert
        var jwt = handler.ReadJsonWebToken(token);

        // subは元のcustomer idからextraClaimsの値へ上書きされる
        Assert.AreEqual("override-subject", jwt.Subject);

        // 追加クレームが反映される
        var roleClaim = jwt.Claims.SingleOrDefault(x => x.Type == "role");
        Assert.IsNotNull(roleClaim);
        Assert.AreEqual("admin", roleClaim.Value);
    }

    private static Customer CreateCustomer()
    {
        return Customer.Restore(
            Guid.NewGuid().ToString(),
            "LoginTest",
            "ログインテスト",
            "Tokyo",
            "Chiyoda",
            "03-1234-5678",
            "LoginTest@example.com",
            "LoginTestUser",
            "hashed-password");
    }

}
