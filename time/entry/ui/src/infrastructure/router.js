import { createRouter, createWebHistory } from 'vue-router'
import auth from "./authState";

import Login from "../modules/Login.vue";

import List from "../modules/List.vue";

const router = createRouter(
    {
        history: createWebHistory(import.meta.env.BASE_URL),
        routes: [
            // Auth routes
            {
                name: "login",
                path: "/login",
                component: Login,
                meta: { public: true }
            },
            {
                path: "/",
                name: "home",
                component: List
            }
        ]
    });


router.beforeEach((to, from, next) =>
{
    if( to.meta.public )
    {
        next();
    }
    else
    {
        if(!auth.isAuthenticated)
        {
            next({name:"login", params: {nextUrl:to.fullPath}});
        }
        else
        {
            next();
        }
    }
});

export default router
