import { Link, useNavigate } from "react-router-dom";
import { motion } from "motion/react";
import "./Home.css";

function Home() {
    const navigate = useNavigate();

    const token = localStorage.getItem("accessToken");
    const usuarioAutenticado = !!token;

    const cardVariants = {
        initial: {
            y: 0,
        },

        hover: {
            y: -8,
            transition: {
                duration: 0.2,
            },
        },
    };

    const contentVariants = {
        initial: {
            opacity: 0,
            height: 0,
            y: -10,
        },

        hover: {
            opacity: 1,
            height: "auto",
            y: 0,
            transition: {
                duration: 0.3,
            },
        },
    };

    function sair() {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("usuarioId");

        navigate("/");
    }

    return (
        <main className="home">
            {/* Navbar */}
            <nav className="navbar navbar-expand-lg navbar-light  border-bottom">
                <div className="container py-2">
                    <Link to="/" className="navbar-brand home-brand">
                        <span className="brand-icon">✓</span>
                        <span>Gerador de Certificados</span>
                    </Link>

                    <div className="d-flex align-items-center gap-2">
                        {usuarioAutenticado ? (
                            <>
                                <Link
                                    to="/cursos"
                                    className="btn btn-outline-primary"
                                >
                                    Meus cursos
                                </Link>

                                <button
                                    type="button"
                                    className="btn btn-primary"
                                    onClick={sair}
                                >
                                    Sair
                                </button>
                            </>
                        ) : (
                            <>
                                <Link
                                    to="/entrar"
                                    className="btn btn-outline-primary"
                                >
                                    Entrar
                                </Link>

                                <Link
                                    to="/cadastrar"
                                    className="btn btn-primary"
                                >
                                    Criar conta
                                </Link>
                            </>
                        )}
                    </div>
                </div>
            </nav>

            {/* Hero */}
            <section className="hero">
                <div className="container">
                    <div className="row align-items-center g-5">
                        <div className="col-lg-6">
                            <div className="hero-icon">
                                <svg
                                    viewBox="0 0 120 120"
                                    fill="none"
                                    xmlns="http://www.w3.org/2000/svg"
                                >
                                    {/* Elemento decorativo esquerdo */}
                                    <path
                                        d="M18 38L11 31"
                                        stroke="currentColor"
                                        strokeWidth="2"
                                        strokeLinecap="round"
                                    />

                                    <path
                                        d="M15 52H7"
                                        stroke="currentColor"
                                        strokeWidth="2"
                                        strokeLinecap="round"
                                    />

                                    {/* Elemento decorativo direito */}
                                    <path
                                        d="M102 38L109 31"
                                        stroke="currentColor"
                                        strokeWidth="2"
                                        strokeLinecap="round"
                                    />

                                    <path
                                        d="M105 52H113"
                                        stroke="currentColor"
                                        strokeWidth="2"
                                        strokeLinecap="round"
                                    />

                                    {/* Documento */}
                                    <rect
                                        x="27"
                                        y="18"
                                        width="66"
                                        height="78"
                                        rx="8"
                                        fill="white"
                                        stroke="currentColor"
                                        strokeWidth="2.5"
                                    />

                                    {/* Parte superior do documento */}
                                    <path
                                        d="M27 38H93"
                                        stroke="currentColor"
                                        strokeWidth="2"
                                    />

                                    {/* Linhas */}
                                    <path
                                        d="M39 29H63"
                                        stroke="currentColor"
                                        strokeWidth="2"
                                        strokeLinecap="round"
                                    />

                                    <path
                                        d="M39 48H72"
                                        stroke="currentColor"
                                        strokeWidth="2"
                                        strokeLinecap="round"
                                        opacity="0.45"
                                    />

                                    <path
                                        d="M39 54H67"
                                        stroke="currentColor"
                                        strokeWidth="2"
                                        strokeLinecap="round"
                                        opacity="0.3"
                                    />

                                    {/* Selo */}
                                    <circle
                                        cx="60"
                                        cy="72"
                                        r="16"
                                        fill="#4f46e5"
                                    />

                                    <circle
                                        cx="60"
                                        cy="72"
                                        r="11"
                                        stroke="white"
                                        strokeWidth="1.5"
                                        opacity="0.8"
                                    />

                                    {/* Check */}
                                    <path
                                        d="M53 72L58 77L68 66"
                                        stroke="white"
                                        strokeWidth="2.5"
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                    />

                                    {/* Fita do certificado */}
                                    <path
                                        d="M50 84L47 105L60 98L73 105L70 84"
                                        fill="#6366f1"
                                        stroke="currentColor"
                                        strokeWidth="2"
                                        strokeLinejoin="round"
                                    />

                                    {/* Pequenos detalhes */}
                                    <circle
                                        cx="24"
                                        cy="72"
                                        r="2"
                                        fill="currentColor"
                                        opacity="0.45"
                                    />

                                    <circle
                                        cx="96"
                                        cy="72"
                                        r="2"
                                        fill="currentColor"
                                        opacity="0.45"
                                    />
                                </svg>
                            </div>
                            <motion.span
                                className="hero-badge"
                                initial={{
                                    opacity: 0,
                                    y: 20
                                }}
                                animate={{
                                    opacity: 1,
                                    y: 0
                                }}
                                transition={{
                                    duration: 0.6
                                }}
                            >
                                {usuarioAutenticado
                                    ? "Área do usuário"
                                    : "Geração de certificados"}
                            </motion.span>

                            <motion.h1
                                className="hero-title"
                                initial={{
                                    opacity: 0,
                                    y: 30
                                }}
                                animate={{
                                    opacity: 1,
                                    y: 0
                                }}
                                transition={{
                                    duration: 0.7,
                                    delay: 0.15
                                }}
                            >
                                {usuarioAutenticado ? (
                                    <>
                                        Gerencie seus
                                        <br />
                                        <span>cursos e certificados.</span>
                                    </>
                                ) : (
                                    <>
                                        Seus certificados,
                                        <br />
                                        <span>simples e profissionais.</span>
                                    </>
                                )}
                            </motion.h1>

                            <motion.p
                                className="hero-description"
                                initial={{
                                    opacity: 0,
                                    y: 25
                                }}
                                animate={{
                                    opacity: 1,
                                    y: 0
                                }}
                                transition={{
                                    duration: 0.7,
                                    delay: 0.3
                                }}
                            >
                                {usuarioAutenticado
                                    ? "Acesse seus cursos, acompanhe as informações e gere certificados de forma rápida e organizada."
                                    : "Crie certificados de forma rápida, acompanhe o processamento e tenha seus documentos organizados em um único lugar."}
                            </motion.p>

                            <motion.div
                                className="hero-actions"
                                initial={{
                                    opacity: 0,
                                    y: 25
                                }}
                                animate={{
                                    opacity: 1,
                                    y: 0
                                }}
                                transition={{
                                    duration: 0.7,
                                    delay: 0.45
                                }}
                            >
                                {usuarioAutenticado ? (
                                    <>
                                        <Link
                                            to="/cursos"
                                            className="btn btn-primary btn-lg"
                                        >
                                            Acessar meus cursos
                                        </Link>

                                        <button
                                            type="button"
                                            className="btn btn-outline-secondary btn-lg"
                                            onClick={sair}
                                        >
                                            Sair da conta
                                        </button>
                                    </>
                                ) : (
                                    <>
                                        <Link
                                            to="/cadastrar"
                                            className="btn btn-primary btn-lg"
                                        >
                                            Começar agora
                                        </Link>

                                        <Link
                                            to="/entrar"
                                            className="btn btn-outline-secondary btn-lg"
                                        >
                                            Já tenho uma conta
                                        </Link>
                                    </>
                                )}
                            </motion.div>
                        </div>

                        <div className="col-lg-6">
                            <motion.div
                                className="certificate-preview"
                                initial={{
                                    opacity: 0,
                                    x: 80,
                                    rotate: 8,
                                    scale: 0.9
                                }}
                                animate={{
                                    opacity: 1,
                                    x: 0,
                                    rotate: [1, 1.5, 1],
                                    scale: [1, 1.015, 1]
                                }}
                                transition={{
                                    opacity: {
                                        duration: 0.8,
                                        delay: 0.2
                                    },
                                    x: {
                                        duration: 1,
                                        delay: 0.2,
                                        ease: "easeOut"
                                    },
                                    rotate: {
                                        duration: 5,
                                        repeat: Infinity,
                                        ease: "easeInOut"
                                    },
                                    scale: {
                                        duration: 5,
                                        repeat: Infinity,
                                        ease: "easeInOut"
                                    }
                                }}
                            >
                                <div className="certificate-header">
                                    <span>GERADOR DE CERTIFICADOS</span>

                                    <span className="certificate-check">
                                        ✓
                                    </span>
                                </div>

                                <div className="certificate-content">
                                    <small>CERTIFICADO DE CONCLUSÃO</small>

                                    <h2>Certificado</h2>

                                    <p>Certificamos que</p>

                                    <strong>João da Silva</strong>

                                    <p>
                                        concluiu o curso de
                                        <br />
                                        <b>Desenvolvimento Web</b>
                                    </p>

                                    <div className="certificate-details">
                                        <div>
                                            <small>CARGA HORÁRIA</small>
                                            <strong>40 horas</strong>
                                        </div>

                                        <div>
                                            <small>CONCLUSÃO</small>
                                            <strong>20/09/2026</strong>
                                        </div>
                                    </div>
                                </div>

                                <div className="certificate-footer">
                                    <span>Gerador de Certificados</span>
                                    <span>✓ Validado</span>
                                </div>
                            </motion.div>
                        </div>
                    </div>
                </div>
            </section>

            {/* Como funciona */}
            <section className="how-it-works">
                <div className="container">
                    <div className="section-heading">
                        <span>
                            {usuarioAutenticado
                                ? "SEU FLUXO DE TRABALHO"
                                : "COMO FUNCIONA"}
                        </span>

                        <h2>
                            {usuarioAutenticado ? (
                                <>
                                    Organize seus certificados
                                    <br />
                                    em poucos passos.
                                </>
                            ) : (
                                <>
                                    Gere seus certificados
                                    <br />
                                    em poucos passos.
                                </>
                            )}
                        </h2>

                        <p>
                            {usuarioAutenticado
                                ? "Gerencie seus cursos, gere certificados e acompanhe o processamento dos seus documentos."
                                : "Um processo simples para criar, acompanhar e baixar seus certificados."}
                        </p>
                    </div>

                    <div className="row g-4 mt-3">
                        <div className="col-md-4">
                            <div className="step-card">
                                <div className="step-number">01</div>

                                <h3>Cadastre seu curso</h3>

                                <p>
                                    Informe o nome, descrição, carga horária
                                    e data de conclusão do curso.
                                </p>
                            </div>
                        </div>

                        <div className="col-md-4">
                            <div className="step-card">
                                <div className="step-number">02</div>

                                <h3>Gere os certificados</h3>

                                <p>
                                    Informe os alunos e solicite a geração
                                    automática dos certificados.
                                </p>
                            </div>
                        </div>

                        <div className="col-md-4">
                            <div className="step-card">
                                <div className="step-number">03</div>

                                <h3>Acompanhe e baixe</h3>

                                <p>
                                    Acompanhe o processamento e tenha os
                                    arquivos disponíveis para download.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            {/* Recursos */}
            <section className="features">
                <div className="container">

                    <div className="section-heading">
                        <span>RECURSOS</span>

                        <h2>
                            Tudo que você precisa
                            <br />
                            em um só lugar.
                        </h2>

                        <p>
                            Acesse as principais ferramentas para gerenciar seus
                            cursos e certificados.
                        </p>
                    </div>

                    <div className="row g-4 mt-3">

                        {/* Cursos */}
                        <div className="col-md-4">
                            <motion.div
                                className={`feature-card ${!usuarioAutenticado
                                    ? "feature-card-locked"
                                    : ""
                                    }`}
                                variants={cardVariants}
                                initial="initial"
                                whileHover="hover"
                            >
                                <div className="feature-card-top">

                                    <div className="feature-icon">
                                        <svg
                                            viewBox="0 0 24 24"
                                            fill="none"
                                            stroke="currentColor"
                                            strokeWidth="1.8"
                                            strokeLinecap="round"
                                            strokeLinejoin="round"
                                        >
                                            <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20" />
                                            <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2Z" />
                                            <path d="M8 6h8" />
                                            <path d="M8 10h8" />
                                        </svg>
                                    </div>

                                    {!usuarioAutenticado && (
                                        <span className="feature-lock">
                                            🔒 Login necessário
                                        </span>
                                    )}

                                </div>

                                <div className="feature-title">
                                    <h3>Cursos</h3>

                                    <span className="feature-dropdown-arrow">
                                        ↓
                                    </span>
                                </div>

                                <motion.div
                                    className="feature-dropdown"
                                    variants={contentVariants}
                                >
                                    <p>
                                        Cadastre novos cursos e consulte as
                                        informações dos cursos registrados.
                                    </p>

                                    {usuarioAutenticado ? (
                                        <Link
                                            to="/cursos"
                                            className="feature-action"
                                        >
                                            Acessar cursos
                                            <span>→</span>
                                        </Link>
                                    ) : (
                                        <Link
                                            to="/entrar"
                                            className="feature-action feature-action-locked"
                                        >
                                            Entrar para acessar
                                            <span>→</span>
                                        </Link>
                                    )}
                                </motion.div>
                            </motion.div>
                        </div>


                        {/* Geração rápida */}
                        <div className="col-md-4">
                            <motion.div
                                className={`feature-card ${!usuarioAutenticado
                                    ? "feature-card-locked"
                                    : ""
                                    }`}
                                variants={cardVariants}
                                initial="initial"
                                whileHover="hover"
                            >
                                <div className="feature-card-top">

                                    <div className="feature-icon">
                                        <svg
                                            viewBox="0 0 24 24"
                                            fill="none"
                                            stroke="currentColor"
                                            strokeWidth="1.8"
                                            strokeLinecap="round"
                                            strokeLinejoin="round"
                                        >
                                            <path d="M12 3v12" />
                                            <path d="m7 10 5 5 5-5" />
                                            <path d="M5 21h14" />
                                            <path d="M5 17h14" />
                                        </svg>
                                    </div>

                                    {!usuarioAutenticado && (
                                        <span className="feature-lock">
                                            🔒 Login necessário
                                        </span>
                                    )}

                                </div>

                                <div className="feature-title">
                                    <h3>Geração rápida</h3>

                                    <span className="feature-dropdown-arrow">
                                        ↓
                                    </span>
                                </div>

                                <motion.div
                                    className="feature-dropdown"
                                    variants={contentVariants}
                                >
                                    <p>
                                        Informe os alunos e solicite a geração
                                        automática dos certificados.
                                    </p>

                                    {usuarioAutenticado ? (
                                        <Link
                                            to="/solicitar-certificado"
                                            className="feature-action"
                                        >
                                            Gerar certificados
                                            <span>→</span>
                                        </Link>
                                    ) : (
                                        <Link
                                            to="/entrar"
                                            className="feature-action feature-action-locked"
                                        >
                                            Entrar para acessar
                                            <span>→</span>
                                        </Link>
                                    )}
                                </motion.div>
                            </motion.div>
                        </div>


                        {/* Certificados */}
                        <div className="col-md-4">
                            <motion.div
                                className={`feature-card ${!usuarioAutenticado
                                    ? "feature-card-locked"
                                    : ""
                                    }`}
                                variants={cardVariants}
                                initial="initial"
                                whileHover="hover"
                            >
                                <div className="feature-card-top">

                                    <div className="feature-icon">
                                        <svg
                                            viewBox="0 0 24 24"
                                            fill="none"
                                            stroke="currentColor"
                                            strokeWidth="1.8"
                                            strokeLinecap="round"
                                            strokeLinejoin="round"
                                        >
                                            <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
                                            <path d="M14 2v6h6" />
                                            <path d="m9 15 2 2 4-4" />
                                        </svg>
                                    </div>

                                    {!usuarioAutenticado && (
                                        <span className="feature-lock">
                                            🔒 Login necessário
                                        </span>
                                    )}

                                </div>

                                <div className="feature-title">
                                    <h3>Certificados</h3>

                                    <span className="feature-dropdown-arrow">
                                        ↓
                                    </span>
                                </div>

                                <motion.div
                                    className="feature-dropdown"
                                    variants={contentVariants}
                                >
                                    <p>
                                        Consulte os certificados gerados e baixe
                                        seus arquivos agrupados.
                                    </p>

                                    {usuarioAutenticado ? (
                                        <Link
                                            to="/certificados"
                                            className="feature-action"
                                        >
                                            Ver certificados
                                            <span>→</span>
                                        </Link>
                                    ) : (
                                        <Link
                                            to="/entrar"
                                            className="feature-action feature-action-locked"
                                        >
                                            Entrar para acessar
                                            <span>→</span>
                                        </Link>
                                    )}
                                </motion.div>
                            </motion.div>
                        </div>

                    </div>
                </div>
            </section>

            {/* CTA */}
            <section className="cta-section">
                <div className="container">
                    <div className="cta">
                        <div>
                            <span>
                                {usuarioAutenticado
                                    ? "CONTINUE SEU TRABALHO"
                                    : "COMECE AGORA"}
                            </span>

                            <h2>
                                {usuarioAutenticado
                                    ? "Pronto para gerenciar seus certificados?"
                                    : "Pronto para gerar seus certificados?"}
                            </h2>

                            <p>
                                {usuarioAutenticado
                                    ? "Acesse seus cursos e continue gerenciando seus certificados."
                                    : "Crie sua conta e comece a organizar seus certificados."}
                            </p>
                        </div>

                        {usuarioAutenticado ? (
                            <Link
                                to="/cursos"
                                className="btn btn-light btn-lg"
                            >
                                Acessar meus cursos
                            </Link>
                        ) : (
                            <Link
                                to="/cadastrar"
                                className="btn btn-light btn-lg"
                            >
                                Criar minha conta
                            </Link>
                        )}
                    </div>
                </div>
            </section>

            {/* Footer */}
            <footer className="footer">
                <div className="container">
                    <div className="footer-content">
                        <div className="footer-brand">
                            <span className="brand-icon">✓</span>
                            <span>Gerador de Certificados</span>
                        </div>

                        <span>
                            © 2026 Gerador de Certificados
                        </span>
                    </div>
                </div>
            </footer>
        </main>
    );


}

export default Home;
