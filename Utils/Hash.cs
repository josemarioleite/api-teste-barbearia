using API.Models.ClientesUsuarios;
using API.Models.Usuarios;

namespace Utils
{
    public class Hash
    {
        public void HasheiaSenha(UsuarioCadastro usuario)
        {
            if(usuario.Senha != null && !string.IsNullOrWhiteSpace(usuario.Senha))
            {
                using (var hmac = new System.Security.Cryptography.HMACSHA512())
                {
                    usuario.SenhaDificuldade = hmac.Key;
                    usuario.SenhaHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(usuario.Senha));
                }
            }
        }

        public void HasheiaSenha(ClienteUsuarioCadastro usuario)
        {
            if (usuario.Senha != null && !string.IsNullOrWhiteSpace(usuario.Senha))
            {
                using (var hmac = new System.Security.Cryptography.HMACSHA512())
                {
                    usuario.SenhaDificuldade = hmac.Key;
                    usuario.SenhaHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(usuario.Senha));
                }
            }
        }

        public void HasheiaSenha(ClienteUsuarioAlteraSenha usuario)
        {
            if (usuario.Senha != null && !string.IsNullOrWhiteSpace(usuario.Senha))
            {
                using (var hmac = new System.Security.Cryptography.HMACSHA512()) {
                    usuario.SenhaDificuldade = hmac.Key;
                    usuario.SenhaHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(usuario.Senha));
                }
            }
        }

        public bool VerificaSenhaHash(Usuario usuario, string senha)
        {
            if(senha != null && !string.IsNullOrWhiteSpace(senha))
            {
                if(usuario.SenhaHash.Length == 64 && usuario.SenhaDificuldade.Length == 128){
                    using (var hmac = new System.Security.Cryptography.HMACSHA512(usuario.SenhaDificuldade))
                    {
                        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
                        for(int i = 0; i < computedHash.Length; i++)
                        {
                            if(computedHash[i] != usuario.SenhaHash[i])
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }else{
                    return false;
                }
            }else{
                return false;
            }
        }

        public bool VerificaSenhaHash(ClienteUsuario usuario, string senha)
        {
            if (senha != null && !string.IsNullOrWhiteSpace(senha))
            {
                if (usuario.SenhaHash.Length == 64 && usuario.SenhaDificuldade.Length == 128)
                {
                    using (var hmac = new System.Security.Cryptography.HMACSHA512(usuario.SenhaDificuldade))
                    {
                        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
                        for (int i = 0; i < computedHash.Length; i++)
                        {
                            if (computedHash[i] != usuario.SenhaHash[i])
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}