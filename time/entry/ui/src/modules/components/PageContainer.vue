<template>
    <div class="mx-auto mt-5 max-w-[600px]">
        <div class="max-w-[300px] mx-auto text-light text-center mb-5">
            <img src="../../assets/logo.png" />
            <h1 class="text-muted text-xl sm:text-2xl">Time sample</h1>
        </div>
        
        <div class="bg-light text-dark rounded shadow p-5">
           
            <div class="text-right" v-if="isAuthenticated">
                <a class="bg-primary text-primary-inv cursor-pointer 
                          hover:bg-primary-alt px-5 py-1 
                          rounded shadow-md" href="/api/logout">Logout {{name}}</a>
            </div>

            <slot/>

        </div>
    </div>
</template>

<script>
import auth from "../../infrastructure/authState";

export default
    {
        computed:
        {
            isAuthenticated()
            {
                return auth.isAuthenticated;
            },
            name()
            {
                return auth.username;
            }
        }
    };
</script>