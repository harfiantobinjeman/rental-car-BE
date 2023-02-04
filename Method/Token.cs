using rentalcar_backend.Models;

// CRYPTO AND AUTHHH
using System.Security.Cryptography;
using System.Text;
using System.Data;

// AUTHHH
using System.Security.Claims;

// DOWNLOAD AUTHHH
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;



namespace rentalcar_backend.Method
{
    public static class Token
    {
        private static IConfiguration _configuration;
        private static string _jwtKey;
        private static string _jwtIssuer;
        private static string _jwtAudience;

        //get appsetting.json
        public static void Init(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtKey = configuration["Jwt:Key"];
            _jwtIssuer = configuration["Jwt:Issuer"];
            _jwtAudience = configuration["Jwt:Audience"];
        }

        //Create JWT Token
        public static string CreateJwtToken(List<Claim> claims, string jwtKey = "", string issuer = "", string audience = "")
        {
            // default value
            if (String.IsNullOrEmpty(jwtKey)) jwtKey = _jwtKey;
            if (String.IsNullOrEmpty(issuer)) issuer = _jwtIssuer;
            if (String.IsNullOrEmpty(audience)) audience = _jwtAudience;

            // create credential/signature for jwtToken
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            SigningCredentials cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // create jwt token
            JwtSecurityToken tokenSettings = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred,
                issuer: issuer,
                audience: audience
            );
            string jwtToken = new JwtSecurityTokenHandler().WriteToken(tokenSettings);

            return jwtToken;
        }


        //Validate JWTtoken
        public static List<Claim> ParseJwtTokenClaim(string jwtToken, string jwtKey = "", string issuer = "", string audience = "")
        {
            // default value
            if (String.IsNullOrEmpty(jwtKey)) jwtKey = _jwtKey;
            if (String.IsNullOrEmpty(issuer)) issuer = _jwtIssuer;
            if (String.IsNullOrEmpty(audience)) audience = _jwtAudience;

            // validate setting
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                // validate signature/credential
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                // validate issuer
                ValidateIssuer = true,
                ValidIssuer = issuer,
                // validate audience
                ValidateAudience = true,
                ValidAudience = audience,
                // validate time
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };

            // get claims
            SecurityToken validatedToken;
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal.Claims.ToList();
        }

    }
}
