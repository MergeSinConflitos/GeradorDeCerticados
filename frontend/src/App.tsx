import { BrowserRouter, Routes, Route } from "react-router-dom";

import Home from "./pages/Home";
import Entrar from "./pages/Entrar";
import Cadastrar from "./pages/Cadastrar";
import Curso from "./pages/Curso";
import SolicitarCertificado from "./pages/SolicitarCertificado";
import Certificados from "./pages/Certificados";


function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/entrar" element={<Entrar />} />
        <Route path="/cadastrar" element={<Cadastrar />} />
        <Route path="/cursos" element={<Curso />} />
        <Route
          path="/solicitar-certificado"
          element={<SolicitarCertificado />}
        />
        <Route
          path="/certificados"
          element={<Certificados />}
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
