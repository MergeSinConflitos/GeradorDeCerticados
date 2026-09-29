import { type FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "./Cadastrar.css";

const API_URL = "http://localhost:5186";

function Cadastrar() {
    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [senha, setSenha] = useState("");
    const [erro, setErro] = useState("");
    const [carregando, setCarregando] = useState(false);

    async function cadastrar(event: FormEvent) {
        event.preventDefault();

        setErro("");
        setCarregando(true);

        try {
            const response = await fetch(`${API_URL}/auth/cadastro`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    email,
                    senha,
                }),
            });

            const data = await response.json();

            if (!response.ok) {
                const mensagem =
                    data.detail ??
                    Object.values(data.errors ?? {})
                        .flat()
                        .join(" ");

                setErro(
                    mensagem || "Não foi possível realizar o cadastro."
                );

                return;
            }

            navigate("/");
        } catch {
            setErro("Não foi possível conectar com a API.");
        } finally {
            setCarregando(false);
        }
    }

    return (
        <main className="auth-page">
            <div className="container">
                <div className="row justify-content-center align-items-center min-vh-100 py-5">
                    <div className="col-lg-10">
                        <div className="auth-card">
                            <div className="row g-0">

                                <div className="col-lg-5 auth-info">
                                    <Link to="/" className="auth-brand">
                                        <span className="brand-icon">✓</span>
                                        <span>Gerador de Certificados</span>
                                    </Link>

                                    <div className="auth-info-content">
                                        <span className="auth-label">
                                            COMECE AGORA
                                        </span>

                                        <h1>
                                            Crie sua conta e organize seus
                                            certificados.
                                        </h1>

                                        <p>
                                            Tenha seus cursos e certificados
                                            organizados em um único lugar.
                                        </p>
                                    </div>
                                </div>

                                <div className="col-lg-7">
                                    <div className="auth-form-container">
                                        <div className="auth-header">
                                            <span className="mobile-brand">
                                                <span className="brand-icon">
                                                    ✓
                                                </span>
                                            </span>

                                            <h2>Criar conta</h2>

                                            <p>
                                                Preencha seus dados para começar.
                                            </p>
                                        </div>

                                        {erro && (
                                            <div
                                                className="alert alert-danger auth-error"
                                                role="alert"
                                            >
                                                {erro}
                                            </div>
                                        )}

                                        <form onSubmit={cadastrar}>
                                            <div className="mb-3">
                                                <label
                                                    htmlFor="email"
                                                    className="form-label"
                                                >
                                                    E-mail
                                                </label>

                                                <input
                                                    id="email"
                                                    type="email"
                                                    className="form-control form-control-lg"
                                                    value={email}
                                                    onChange={(event) =>
                                                        setEmail(event.target.value)
                                                    }
                                                    placeholder="seu@email.com"
                                                    required
                                                />
                                            </div>

                                            <div className="mb-4">
                                                <label
                                                    htmlFor="senha"
                                                    className="form-label"
                                                >
                                                    Senha
                                                </label>

                                                <input
                                                    id="senha"
                                                    type="password"
                                                    className="form-control form-control-lg"
                                                    value={senha}
                                                    onChange={(event) =>
                                                        setSenha(event.target.value)
                                                    }
                                                    placeholder="Digite sua senha"
                                                    required
                                                />
                                            </div>

                                            <button
                                                type="submit"
                                                className="btn btn-primary btn-lg w-100 auth-submit"
                                                disabled={carregando}
                                            >
                                                {carregando
                                                    ? "Criando conta..."
                                                    : "Criar conta"}
                                            </button>
                                        </form>

                                        <div className="auth-footer">
                                            <span>
                                                Já possui uma conta?
                                            </span>{" "}
                                            <Link to="/entrar">
                                                Entrar
                                            </Link>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </main>
    );
}

export default Cadastrar;