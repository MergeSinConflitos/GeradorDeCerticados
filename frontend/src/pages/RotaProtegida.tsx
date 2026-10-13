
import { Navigate, Outlet } from "react-router-dom";

function RotaProtegida() {
    const token = localStorage.getItem("accessToken");

    if (!token) {
        return <Navigate to="/entrar" replace />;
    }

    return <Outlet />;
}

export default RotaProtegida;
