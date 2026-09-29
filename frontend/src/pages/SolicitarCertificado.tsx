import { useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import { apiFetch } from "../services/api";
import "./Curso.css";
import "./SolicitarCertificado.css";

type SolicitarCertificadoRequest = {
    nomesAlunos: string[];
};

function SolicitarCertificado() {
    const [cursoId, setCursoId] = useState("");
    const [nomeAluno, setNomeAluno] = useState("");
    const [nomesAlunos, setNomesAlunos] = useState<string[]>([]);

    const [status, setStatus] = useState("");
    const [consultandoStatus, setConsultandoStatus] = useState(false);

    const [processando, setProcessando] = useState(false);
    const [erro, setErro] = useState("");
    const [sucesso, setSucesso] = useState("");

    async function solicitarCertificado() {
        setErro("");
        setSucesso("");
        setStatus("");
        setProcessando(true);

        try {
            const request: SolicitarCertificadoRequest = {
                nomesAlunos,
            };

            const response = await apiFetch(
                `/cursos/${cursoId}/certificados`,
                {
                    method: "POST",
                    body: JSON.stringify(request),
                }
            );

            const data = await response.json();

            if (!response.ok) {
                const mensagem =
                    data.detail ??
                    Object.values(data.errors ?? {})
                        .flat()
                        .join(" ");

                throw new Error(
                    mensagem || "Não foi possível fazer a solicitação."
                );
            }

            setSucesso("Solicitação feita com sucesso.");

            setNomeAluno("");
            setNomesAlunos([]);
        } catch (error) {
            setErro(
                error instanceof Error
                    ? error.message
                    : "Não foi possível fazer a solicitação."
            );
        } finally {
            setProcessando(false);
        }
    }

    function adicionarAluno() {
        if (!nomeAluno.trim()) {
            return;
        }

        setNomesAlunos([
            ...nomesAlunos,
            nomeAluno.trim(),
        ]);

        setNomeAluno("");
    }

    async function consultarStatus() {
        setErro("");
        setStatus("");
        setConsultandoStatus(true);

        try {
            const response = await apiFetch(
                `/cursos/${cursoId}/status`,
                {
                    method: "GET",
                }
            );

            const data = await response.json();

            if (!response.ok) {
                const mensagem =
                    data.detail ??
                    Object.values(data.errors ?? {})
                        .flat()
                        .join(" ");

                throw new Error(
                    mensagem || "Não foi possível consultar o status."
                );
            }

            setStatus(data.status);
        } catch (error) {
            setErro(
                error instanceof Error
                    ? error.message
                    : "Não foi possível consultar o status."
            );
        } finally {
            setConsultandoStatus(false);
        }
    }

    function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        solicitarCertificado();
    }

    return (
        <main className="curso-page">
            <section className="curso-section">
                <div className="container">

                    {/* Cabeçalho */}
                    <div className="curso-header">
                        <span className="curso-badge">
                            GERAÇÃO DE CERTIFICADOS
                        </span>

                        <h1>Solicite seus certificados</h1>

                        <p>
                            Informe o curso e os alunos para solicitar a
                            geração dos certificados.
                        </p>
                    </div>

                    {/* Solicitação */}
                    <div className="curso-form-card">
                        <div className="curso-card-header">
                            <span className="curso-card-icon">
                                +
                            </span>

                            <div>
                                <h2>Solicitar certificados</h2>

                                <p>
                                    Adicione os alunos que receberão os
                                    certificados.
                                </p>
                            </div>
                        </div>

                        <form onSubmit={handleSubmit}>

                            {/* Curso */}
                            <div className="mb-4">
                                <label
                                    htmlFor="cursoId"
                                    className="form-label"
                                >
                                    ID do curso
                                </label>

                                <input
                                    id="cursoId"
                                    type="text"
                                    className="form-control"
                                    value={cursoId}
                                    onChange={(event) =>
                                        setCursoId(event.target.value)
                                    }
                                    placeholder="Informe o ID do curso"
                                    disabled={processando}
                                />
                            </div>

                            {/* Aluno */}
                            <div className="mb-4">
                                <label
                                    htmlFor="nomeAluno"
                                    className="form-label"
                                >
                                    Nome do aluno
                                </label>

                                <div className="input-group">
                                    <input
                                        id="nomeAluno"
                                        type="text"
                                        className="form-control"
                                        value={nomeAluno}
                                        onChange={(event) =>
                                            setNomeAluno(
                                                event.target.value
                                            )
                                        }
                                        placeholder="Informe o nome do aluno"
                                        disabled={processando}
                                    />

                                    <button
                                        type="button"
                                        className="btn btn-outline-primary"
                                        onClick={adicionarAluno}
                                        disabled={
                                            processando ||
                                            !nomeAluno.trim()
                                        }
                                    >
                                        Adicionar
                                    </button>
                                </div>
                            </div>

                            {/* Alunos */}
                            {nomesAlunos.length > 0 && (
                                <div className="mb-4">
                                    <div className="curso-result-description">
                                        <span>
                                            ALUNOS ADICIONADOS
                                        </span>
                                    </div>

                                    <ul className="list-group mt-2">
                                        {nomesAlunos.map(
                                            (nome, index) => (
                                                <li
                                                    key={index}
                                                    className="list-group-item"
                                                >
                                                    {nome}
                                                </li>
                                            )
                                        )}
                                    </ul>
                                </div>
                            )}

                            {erro && (
                                <div
                                    className="alert alert-danger mt-4"
                                    role="alert"
                                >
                                    {erro}
                                </div>
                            )}

                            {sucesso && (
                                <div
                                    className="alert alert-success mt-4"
                                    role="alert"
                                >
                                    {sucesso}
                                </div>
                            )}

                            {status && (
                                <div
                                    className="alert alert-info mt-4"
                                    role="alert"
                                >
                                    Status da solicitação:{" "}
                                    <strong>{status}</strong>
                                </div>
                            )}

                            {/* Ações */}
                            <div className="d-flex gap-2 flex-wrap mt-4">
                                <button
                                    type="submit"
                                    className="btn btn-primary btn-lg"
                                    disabled={
                                        processando ||
                                        consultandoStatus ||
                                        !cursoId.trim() ||
                                        nomesAlunos.length === 0
                                    }
                                >
                                    {processando
                                        ? "Solicitando..."
                                        : "Solicitar certificados"}
                                </button>

                                <button
                                    type="button"
                                    className="btn btn-outline-primary btn-lg"
                                    onClick={consultarStatus}
                                    disabled={
                                        !cursoId.trim() ||
                                        processando ||
                                        consultandoStatus
                                    }
                                >
                                    {consultandoStatus
                                        ? "Consultando..."
                                        : "Ver status"}
                                </button>
                            </div>
                        </form>
                    </div>

                    {/* Voltar */}
                    <Link
                        to="/"
                        className="btn btn-outline-secondary mt-4"
                    >
                        Voltar
                    </Link>
                </div>
            </section>
        </main>
    );
}

export default SolicitarCertificado;