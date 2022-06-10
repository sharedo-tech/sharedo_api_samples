<template>
    <PageContainer>
        <div class="text-center">
            <p>You need to login to your sharedo instance</p>

            <button class="bg-primary text-primary-inv hover:bg-primary-alt 
                        mt-5 px-5 py-1 rounded shadow-md" 
                    @click="login">
                Login
            </button>
        </div>
    </PageContainer>
</template>

<script>
import PageContainer from "./components/PageContainer.vue";

export default
    {
        name: "Login",
        components: { PageContainer },
        props: [],
        data()
        {
            let nextUrl = "/";
            if (this.$route && this.$route.params && this.$route.params.nextUrl)
            {
                nextUrl = this.$route.params.nextUrl;
            }

            return {
                nextUrl: nextUrl
            }
        },
        methods:
        {
            login(e)
            {
                e.preventDefault();
                let nextUrl = encodeURIComponent(this.nextUrl);
                window.location.href = `/api/login?returnUrl=${nextUrl}`;
            }
        }
    };
</script>
