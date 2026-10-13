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
    //Estado da tela Curso
    type TelaCurso = "lista" | "cadastro" | "consulta";
    const [telaAtual, setTelaAtual] = useState<TelaCurso>("lista");

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
    const [cursos, setCursos] = useState<Curso[]>([]);

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

    async function listarCursos() {
        setErroConsulta("");
        setCurso(null);
        setCursos([]);
        setConsultando(true);

        try {
            const response = await apiFetch("/Api/cursos");

            const data = await response.json();

            if (!response.ok) {
                throw new Error(
                    data.detail ?? "Não foi possível listar os cursos."
                );
            }

            setCursos(data);
        } catch (error) {
            setErroConsulta(
                error instanceof Error
                    ? error.message
                    : "Não foi possível listar os cursos."
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

                        <h1>Gerenciamento de cursos</h1>

                        <p>
                            Cadastre, consulte e acompanhe os cursos registrados
                            na plataforma.
                        </p>
                    </div>

                    {/* Navegação */}
                    <div className="curso-form-card mb-4">
                        <div className="d-flex flex-wrap gap-2">
                            <button
                                type="button"
                                className={`btn ${telaAtual === "lista"
                                    ? "btn-primary"
                                    : "btn-outline-primary"
                                    }`}
                                onClick={() => {
                                    setTelaAtual("lista");
                                    listarCursos();
                                }}
                            >
                                Listar cursos
                            </button>

                            <button
                                type="button"
                                className={`btn ${telaAtual === "cadastro"
                                    ? "btn-primary"
                                    : "btn-outline-primary"
                                    }`}
                                onClick={() => {
                                    setTelaAtual("cadastro");
                                    setErro("");
                                    setSucesso("");
                                }}
                            >
                                + Cadastrar curso
                            </button>

                            <button
                                type="button"
                                className={`btn ${telaAtual === "consulta"
                                    ? "btn-primary"
                                    : "btn-outline-primary"
                                    }`}
                                onClick={() => {
                                    setTelaAtual("consulta");
                                    setErroConsulta("");
                                    setCurso(null);
                                }}
                            >
                                Consultar por ID
                            </button>
                        </div>
                    </div>

                    {/* Listagem */}
                    {telaAtual === "lista" && (
                        <div className="curso-consulta-card">
                            <div className="curso-card-header">
                                <span className="curso-card-icon">≡</span>

                                <div>
                                    <h2>Cursos cadastrados</h2>
                                    <p>
                                        Confira os cursos disponíveis na plataforma.
                                    </p>
                                </div>
                            </div>

                            <div className="d-flex justify-content-end mb-4">
                                <button
                                    type="button"
                                    className="btn btn-outline-primary"
                                    onClick={listarCursos}
                                    disabled={consultando}
                                >
                                    {consultando
                                        ? "Carregando..."
                                        : "Atualizar listagem"}
                                </button>
                            </div>

                            {erroConsulta && (
                                <div className="alert alert-danger" role="alert">
                                    {erroConsulta}
                                </div>
                            )}

                            {!consultando && !erroConsulta && cursos.length === 0 && (
                                <div className="alert alert-info" role="status">
                                    Nenhum curso cadastrado.
                                </div>
                            )}

                            {cursos.length > 0 && (
                                <div className="table-responsive">
                                    <table className="table table-hover align-middle">
                                        <thead>
                                            <tr>
                                                <th>Curso</th>
                                                <th>Carga horária</th>
                                                <th>Conclusão</th>
                                                <th className="text-end">Ações</th>
                                            </tr>
                                        </thead>

                                        <tbody>
                                            {cursos.map((item) => (
                                                <tr key={item.id}>
                                                    <td>
                                                        <strong>{item.nome}</strong>
                                                        <div className="text-muted small">
                                                            {item.descricao ||
                                                                "Sem descrição"}
                                                        </div>
                                                    </td>

                                                    <td>
                                                        {item.cargaHoraria} horas
                                                    </td>

                                                    <td>
                                                        {formatarData(
                                                            item.dataConclusao
                                                        )}
                                                    </td>

                                                    <td className="text-end">
                                                        <button
                                                            type="button"
                                                            className="btn btn-sm btn-outline-primary"
                                                            onClick={() => {
                                                                setCursoId(item.id);
                                                                setCurso(null);
                                                                setErroConsulta("");
                                                                setTelaAtual("consulta");
                                                                obterCurso(item.id);
                                                            }}
                                                        >
                                                            Ver detalhes
                                                        </button>
                                                    </td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                </div>
                            )}
                        </div>
                    )}

                    {/* Cadastro */}
                    {telaAtual === "cadastro" && (
                        <div className="curso-form-card">
                            <div className="curso-card-header">
                                <span className="curso-card-icon">+</span>

                                <div>
                                    <h2>Cadastrar curso</h2>
                                    <p>Preencha os dados do novo curso.</p>
                                </div>
                            </div>

                            <form onSubmit={handleSubmitCadastro}>
                                <div className="mb-4">
                                    <label htmlFor="nome" className="form-label">
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
                                        required
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
                                                required
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
                                            required
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
                                        role="status"
                                    >
                                        {sucesso}
                                        <p className="mb-0 mt-2">
                                            ID do curso: <code>{cursoId}</code>
                                        </p>
                                    </div>
                                )}

                                <div className="d-flex flex-wrap gap-2 mt-4">
                                    <button
                                        type="submit"
                                        className="btn btn-primary btn-lg"
                                        disabled={carregando}
                                    >
                                        {carregando
                                            ? "Cadastrando..."
                                            : "Cadastrar curso"}
                                    </button>

                                    <button
                                        type="button"
                                        className="btn btn-outline-secondary btn-lg"
                                        onClick={() => setTelaAtual("lista")}
                                    >
                                        Voltar para listagem
                                    </button>
                                </div>
                            </form>
                        </div>
                    )}

                    {/* Consulta por ID */}
                    {telaAtual === "consulta" && (
                        <div className="curso-consulta-card">
                            <div className="curso-card-header">
                                <span className="curso-card-icon">⌕</span>

                                <div>
                                    <h2>Consultar curso</h2>
                                    <p>
                                        Informe o ID ou selecione um curso na
                                        listagem para consultar seus detalhes.
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
                                            placeholder="Informe o ID do curso"
                                            disabled={consultando}
                                            required
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
                                    className="alert alert-danger mt-4"
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
                                                {formatarData(curso.dataConclusao)}
                                            </strong>
                                        </div>
                                    </div>

                                    <div className="curso-result-id">
                                        <span>ID DO CURSO</span>
                                        <code>{curso.id}</code>
                                    </div>
                                </div>
                            )}

                            <button
                                type="button"
                                className="btn btn-outline-secondary mt-4"
                                onClick={() => setTelaAtual("lista")}
                            >
                                Voltar para listagem
                            </button>
                        </div>
                    )}

                    <div className="mt-4">
                        <Link to="/" className="btn btn-outline-secondary">
                            Voltar ao início
                        </Link>
                    </div>
                </div>
            </section>
        </main>
    );
}

export default Curso;