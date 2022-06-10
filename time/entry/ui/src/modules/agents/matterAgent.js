import api from "../../infrastructure/fetch";

function getMyMatters()
{
    return api.get("/api/matters");
}

const agent =
{
    getMyMatters: getMyMatters
};

export default agent;
