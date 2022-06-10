<script setup>
import PageContainer from "./components/PageContainer.vue";
</script>

<template>
    <PageContainer>

        <h2 class="text-xl">Book time</h2>
        <p>Choose a recent matter to book time to</p>

        <div v-if="error" class="mt-1 bg-red-600 rounded text-white p-3">{{error}}</div>

        <div v-if="loading" class="mt-1">Loading...</div>

        <template v-if="!loading">
            <div v-for="m in matters" :key="m.id"
                 class="mt-1 bg-gray-100 p-3 rounded cursor-pointer hover:bg-gray-200"
                 @click.stop.prevent="e => startCapture(e, m)">
                <h3>{{m.title}}</h3>
                <div class="text-muted">{{m.reference}}</div>
                <div class="text-right mt-1 text-sm">
                    <a class="text-primary underline" :href="m.url" target="_blank" @click.stop="">Open in Sharedo</a>
                </div>
            </div>
        </template>

    </PageContainer>
</template>

<script>
import bladeManager from "../infrastructure/blades";

import matters from "./agents/matterAgent";

import AddTimeBlade from "./blades/AddTimeBlade.vue";

export default
    {
        data()
        {
            return {
                loaded: false,
                loading: false,
                error: null,
                matters: []
            };
        },
        created()
        {
            this.loadMatters();
            window.model = this;
        },
        methods:
        {
            loadMatters()
            {
                if (this.loading) return;
                this.loading = true;
                this.error = null;

                matters.getMyMatters().then(
                    response =>
                    {
                        this.matters = response;
                        this.loading = false;
                        this.loaded = true;
                    },
                    err =>
                    {
                        this.matters = [];
                        this.error = err;
                        this.loading = false;
                        this.loaded = true;
                    });
            },
            startCapture(e, workItem)
            {
                bladeManager.openBlade(AddTimeBlade, {
                    workItemId: workItem.id,
                    reference: workItem.reference,
                    title: workItem.title
                });
            }
        }
    };

</script>