import api from "../../infrastructure/fetch";

function getCategories()
{
    return api.get("/api/time/categories");
}

function getCapture(category, workItemId)
{
    return api.get(`/api/time/capture/${category}/${workItemId}`);
}

function sendTime(workItemId, category, minutes, segments)
{
    var request = 
    {
        workItemId: workItemId,
        categorySystemName: category,
        seconds: minutes * 60,
        segments: segments
    };

    return api.post("/api/time/", request);
}

const agent =
{
    getCategories: getCategories,
    getCapture: getCapture,
    sendTime: sendTime
};

export default agent;
