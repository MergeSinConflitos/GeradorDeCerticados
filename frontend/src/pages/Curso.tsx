import { useState, type FormEvent } from "react";
import { apiFetch } from "../services/api";
import "./Curso.css";
import { Link } from "react-router-dom";

type Curso = {
    id: string;
    nome: string;
    descricao: string | null;
    cargaHoraria: number;
    dataConclusao: string;
};

function Curso() {
    // Cadastro
    const [nome, setNome] = useState("");
    const [descricao, setDescricao] = useState("");
    const [cargaHoraria, setCargaHoraria] = useState("");
    const [dataConclusao, setDataConclusao] = useState("");

    const [carregando, setCarregando] = useState(false);
    const [erro, setErro] = useState("");
    const [sucesso, setSucesso] = useState("");

    // Consulta
    const [cursoId, setCursoId] = useState("");
    const [curso, setCurso] = useState<Curso | null>(null);
    const [consultando, setConsultando] = useState(false);
    const [erroConsulta, setErroConsulta] = useState("");

    async function cadastrarCurso() {
        setErro("");
        setSucesso("");
        setCarregando(true);

        try {
            const response = await apiFetch("/Api/cursos", {
                method: "POST",
                body: JSON.stringify({
                    nome,
                    descricao: descricao || null,
                    cargaHoraria: Number(cargaHoraria),
                    dataConclusao: `${dataConclusao}T00:00:00.000Z`,
                }),
            });

            const data = await response.json();


            if (!response.ok) {
                const mensagem =
                    data.detail ??
                    Object.values(data.errors ?? {})
                        .flat()
                        .join(" ");

                throw new Error(
                    mensagem || "Não foi possível cadastrar o curso."
                );
            }

            setSucesso("Curso cadastrado com sucesso.");
            setCursoId(data.cursoId);
            setNome("");
            setDescricao("");
            setCargaHoraria("");
            setDataConclusao("");
        } catch (error) {
            setErro(
                error instanceof Error
                    ? error.message
                    : "Não foi possível cadastrar o curso."
            );
        } finally {
            setCarregando(false);
        }
    }

    async function obterCurso(id: string) {
        setErroConsulta("");
        setCurso(null);
        setConsultando(true);

        try {
            const response = await apiFetch(`/Api/cursos/${id}`);

            const data = await response.json();

            if (!response.ok) {
                const mensagem =
                    data.detail ??
                    Object.values(data.errors ?? {})
                        .flat()
                        .join(" ");

                throw new Error(
                    mensagem || "Não foi possível consultar o curso."
                );
            }

            setCurso(data);
        } catch (error) {
            setErroConsulta(
                error instanceof Error
                    ? error.message
                    : "Não foi possível consultar o curso."
            );
        } finally {
            setConsultando(false);
        }
    }

    function handleSubmitCadastro(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        cadastrarCurso();
    }

    function handleSubmitConsulta(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();

        if (!cursoId.trim()) {
            setErroConsulta("Informe o ID do curso.");
            setCurso(null);
            return;
        }

        obterCurso(cursoId.trim());
    }

    function formatarData(data: string) {
        return new Date(data).toLocaleDateString("pt-BR");
    }

    return (
        <main className="curso-page">
            <section className="curso-section">
                <div className="container">

                    {/* Cabeçalho */}
                    <div className="curso-header">
                        <span className="curso-badge">
                            GERENCIAMENTO DE CURSOS
                        </span>

                        <h1>Gerencie seus cursos</h1>

                        <p>
                            Cadastre novos cursos e consulte as informações
                            dos cursos já registrados.
                        </p>
                    </div>

                    {/* Cadastro */}
                    <div className="curso-form-card">
                        <div className="curso-card-header">
                            <span className="curso-card-icon">+</span>

                            <div>
                                <h2>Cadastrar curso</h2>

                                <p>
                                    Informe os dados do novo curso.
                                </p>
                            </div>
                        </div>

                        <form onSubmit={handleSubmitCadastro}>
                            <div className="mb-4">
                                <label
                                    htmlFor="nome"
                                    className="form-label"
                                >
                                    Nome do curso
                                </label>

                                <input
                                    id="nome"
                                    type="text"
                                    className="form-control"
                                    value={nome}
                                    onChange={(event) =>
                                        setNome(event.target.value)
                                    }
                                    placeholder="Ex.: Desenvolvimento Web"
                                    disabled={carregando}
                                />
                            </div>

                            <div className="mb-4">
                                <label
                                    htmlFor="descricao"
                                    className="form-label"
                                >
                                    Descrição
                                </label>

                                <textarea
                                    id="descricao"
                                    className="form-control"
                                    rows={4}
                                    value={descricao}
                                    onChange={(event) =>
                                        setDescricao(event.target.value)
                                    }
                                    placeholder="Descreva brevemente o curso..."
                                    disabled={carregando}
                                />
                            </div>

                            <div className="row g-4">
                                <div className="col-md-6">
                                    <label
                                        htmlFor="cargaHoraria"
                                        className="form-label"
                                    >
                                        Carga horária
                                    </label>

                                    <div className="input-group">
                                        <input
                                            id="cargaHoraria"
                                            type="number"
                                            min="1"
                                            className="form-control"
                                            value={cargaHoraria}
                                            onChange={(event) =>
                                                setCargaHoraria(
                                                    event.target.value
                                                )
                                            }
                                            placeholder="40"
                                            disabled={carregando}
                                        />

                                        <span className="input-group-text">
                                            horas
                                        </span>
                                    </div>
                                </div>

                                <div className="col-md-6">
                                    <label
                                        htmlFor="dataConclusao"
                                        className="form-label"
                                    >
                                        Data de conclusão
                                    </label>

                                    <input
                                        id="dataConclusao"
                                        type="date"
                                        className="form-control"
                                        value={dataConclusao}
                                        onChange={(event) =>
                                            setDataConclusao(
                                                event.target.value
                                            )
                                        }
                                        disabled={carregando}
                                    />
                                </div>
                            </div>

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
                                    <h6>O ID do curso é {cursoId}</h6>
                                </div>
                            )}

                            <button
                                type="submit"
                                className="btn btn-primary btn-lg curso-submit"
                                disabled={carregando}
                            >
                                {carregando
                                    ? "Cadastrando..."
                                    : "Cadastrar curso"}
                            </button>
                        </form>
                    </div>

                    {/* Consulta */}
                    <div className="curso-consulta-card">
                        <div className="curso-card-header">
                            <span className="curso-card-icon">⌕</span>

                            <div>
                                <h2>Consultar curso</h2>

                                <p>
                                    Informe o ID para consultar os dados de
                                    um curso.
                                </p>
                            </div>
                        </div>

                        <form onSubmit={handleSubmitConsulta}>
                            <div className="consulta-form">
                                <div className="consulta-input">
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
                                        placeholder="Ex.: 0198..."
                                        disabled={consultando}
                                    />
                                </div>

                                <button
                                    type="submit"
                                    className="btn btn-primary consulta-submit"
                                    disabled={consultando}
                                >
                                    {consultando
                                        ? "Consultando..."
                                        : "Consultar curso"}
                                </button>
                            </div>
                        </form>

                        {erroConsulta && (
                            <div
                                className="alert alert-danger mt-4 mb-0"
                                role="alert"
                            >
                                {erroConsulta}
                            </div>
                        )}

                        {curso && (
                            <div className="curso-result-card">
                                <div className="curso-result-header">
                                    <div>
                                        <span className="curso-result-badge">
                                            CURSO ENCONTRADO
                                        </span>

                                        <h3>{curso.nome}</h3>
                                    </div>

                                    <span className="curso-result-check">
                                        ✓
                                    </span>
                                </div>

                                <div className="curso-result-description">
                                    <span>Descrição</span>

                                    <p>
                                        {curso.descricao ||
                                            "Nenhuma descrição informada."}
                                    </p>
                                </div>

                                <div className="curso-result-details">
                                    <div>
                                        <span>CARGA HORÁRIA</span>
                                        <strong>
                                            {curso.cargaHoraria} horas
                                        </strong>
                                    </div>

                                    <div>
                                        <span>CONCLUSÃO</span>
                                        <strong>
                                            {formatarData(
                                                curso.dataConclusao
                                            )}
                                        </strong>
                                    </div>
                                </div>

                                <div className="curso-result-id">
                                    <span>ID DO CURSO</span>

                                    <code>{curso.id}</code>
                                </div>
                            </div>
                        )}
                    </div>
                    <Link to="/" className="btn btn-outline-secondary">
                        Voltar
                    </Link>
                </div>
            </section>
        </main>
    );
}

export default Curso;