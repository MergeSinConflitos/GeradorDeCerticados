import { useState, type FormEvent } from "react";
import { apiFetch } from "../services/api";
import { Link } from "react-router-dom";
import "./Curso.css";
import "./Certificados.css";

type Certificado = {
    id: string;
    cursoId: string;
    solicitacaoCertificadoId: string;
    nomeAluno: string;
    caminhoArquivo: string | null;
    dataDeGeracao: string | null;
    status: string;
};

function Certificados() {
    const [cursoId, setCursoId] = useState("");
    const [certificados, setCertificados] = useState<Certificado[]>([]);
    const [consultando, setConsultando] = useState(false);
    const [erro, setErro] = useState("");

    async function listarCertificados() {
        setErro("");
        setCertificados([]);
        setConsultando(true);

        try {
            const response = await apiFetch(
                `/cursos/${cursoId}/certificados`
            );

            const data = await response.json();

            if (!response.ok) {
                const mensagem =
                    data.detail ??
                    Object.values(data.errors ?? {})
                        .flat()
                        .join(" ");

                throw new Error(
                    mensagem || "Não foi possível listar os certificados."
                );
            }

            setCertificados(data);
        } catch (error) {
            setErro(
                error instanceof Error
                    ? error.message
                    : "Não foi possível listar os certificados."
            );
        } finally {
            setConsultando(false);
        }
    }

    async function baixarCertificados() {
        setErro("");

        try {
            const response = await apiFetch(
                `/cursos/${cursoId}/certificados/download`
            );

            if (!response.ok) {
                const data = await response.json();

                const mensagem =
                    data.detail ??
                    Object.values(data.errors ?? {})
                        .flat()
                        .join(" ");

                throw new Error(
                    mensagem || "Não foi possível baixar os certificados."
                );
            }

            const blob = await response.blob();

            const url = window.URL.createObjectURL(blob);

            const link = document.createElement("a");

            link.href = url;
            link.download = `certificados-${cursoId}.zip`;

            document.body.appendChild(link);

            link.click();

            link.remove();

            window.URL.revokeObjectURL(url);
        } catch (error) {
            setErro(
                error instanceof Error
                    ? error.message
                    : "Não foi possível baixar os certificados."
            );
        }
    }


    function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        listarCertificados();
    }


    return (
        <main className="curso-page">
            <section className="curso-section">
                <div className="container">

                    {/* Cabeçalho */}
                    <div className="curso-header">
                        <span className="curso-badge">
                            GERENCIAMENTO DE CERTIFICADOS
                        </span>

                        <h1>Seus certificados</h1>

                        <p>
                            Consulte os certificados gerados para um curso
                            e acompanhe as informações dos alunos.
                        </p>
                    </div>

                    {/* Consulta */}
                    <div className="curso-form-card">
                        <div className="curso-card-header">
                            <span className="curso-card-icon">
                                🔎
                            </span>

                            <div>
                                <h2>Consultar certificados</h2>

                                <p>
                                    Informe o ID do curso para listar os
                                    certificados gerados.
                                </p>
                            </div>
                        </div>

                        <form onSubmit={handleSubmit}>
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
                                    disabled={consultando}
                                />
                            </div>

                            {erro && (
                                <div
                                    className="alert alert-danger mt-4"
                                    role="alert"
                                >
                                    {erro}
                                </div>
                            )}

                            <div className="d-flex gap-2 flex-wrap">
                                <button
                                    type="submit"
                                    className="btn btn-primary btn-lg"
                                    disabled={
                                        consultando ||
                                        !cursoId.trim()
                                    }
                                >
                                    {consultando
                                        ? "Consultando..."
                                        : "Listar certificados"}
                                </button>

                                <button
                                    type="button"
                                    className="btn btn-outline-primary btn-lg"
                                    onClick={baixarCertificados}
                                    disabled={!cursoId.trim()}
                                >
                                    Baixar certificados
                                </button>
                            </div>
                        </form>
                    </div>

                    {/* Listagem */}
                    {certificados.length > 0 && (
                        <div className="certificados-lista">

                            {/* Cabeçalho da listagem */}
                            <div className="curso-result-description">
                                <div>
                                    <span>CERTIFICADOS</span>

                                    <h2>
                                        Certificados gerados
                                    </h2>

                                    <p>
                                        Certificados disponíveis para este curso.
                                    </p>
                                </div>

                                <strong>
                                    {certificados.length}
                                </strong>
                            </div>

                            {/* Cards */}
                            <div className="row">

                                {certificados.map((certificado) => (
                                    <div
                                        className="col-md-6"
                                        key={certificado.id}
                                    >
                                        <div className="certificado-card">

                                            {/* Cabeçalho do certificado */}
                                            <div className="certificado-card-header">
                                                <div>
                                                    <span>
                                                        CERTIFICADO
                                                    </span>

                                                    <h3>
                                                        {certificado.nomeAluno}
                                                    </h3>
                                                </div>

                                                <span className="certificado-check">
                                                    ✓
                                                </span>
                                            </div>

                                            {/* Informações principais */}
                                            <div className="certificado-details">

                                                <div>
                                                    <span>
                                                        STATUS
                                                    </span>

                                                    <strong>
                                                        {certificado.status}
                                                    </strong>
                                                </div>

                                                <div>
                                                    <span>
                                                        DATA DE GERAÇÃO
                                                    </span>

                                                    <strong>
                                                        {certificado.dataDeGeracao
                                                            ? new Date(
                                                                certificado.dataDeGeracao
                                                            ).toLocaleDateString(
                                                                "pt-BR"
                                                            )
                                                            : "Ainda não gerado"}
                                                    </strong>
                                                </div>

                                            </div>

                                            {/* ID */}
                                            <div className="certificado-id">
                                                <span>
                                                    ID DO CERTIFICADO
                                                </span>

                                                <code>
                                                    {certificado.id}
                                                </code>
                                            </div>

                                            {/* Arquivo */}
                                            {certificado.caminhoArquivo && (
                                                <div className="certificado-arquivo">
                                                    <span>
                                                        ARQUIVO
                                                    </span>

                                                    <p>
                                                        {certificado.caminhoArquivo}
                                                    </p>
                                                </div>
                                            )}

                                        </div>
                                    </div>
                                ))}

                            </div>
                        </div>
                    )}

                    {/* Nenhum certificado */}
                    {!consultando &&
                        !erro &&
                        certificados.length === 0 && (
                            <div className="certificados-vazio">
                                Nenhum certificado encontrado para este
                                curso.
                            </div>
                        )}

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

export default Certificados