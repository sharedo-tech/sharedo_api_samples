<script setup>
import DropdownSegment from "./DropdownSegment.vue";
</script>

<template>
    <div class="px-4 pb-4">
        <h2 class="text-xl mb-4">Add time entry</h2>
        <div class="mb-4">
            <div class="font-bold">To matter:</div>
            <div class="mt-1">{{reference}}</div>
            <div class="mt-1 text-ellipsis whitespace-nowrap overflow-hidden">{{title}}</div>
        </div>

        <div class="mb-2" v-if="loadingCategories">Loading time categories...</div>

        <div class="mb-2" v-if="!loadingCategories">
            <label for="category" class="font-bold block">Category:</label>
            <select id="category"
                    class="mt-1 bg-white border border-grey-700 text-gray-700
                    focus:outline-none focus:border-primary
                    focus:shadow-outline w-full p-2"
                    v-model="selectedCategory" @change="onCategoryChanged">
                <option v-for="c in categories" :key="c.systemName" :value="c.systemName">{{c.name}}</option>
            </select>
        </div>

        <div class="mb-2" v-if="loadingSegments">Loading capture segments...</div>

        <template v-if="!loadingSegments">
            <div class="mb-2" v-for="seg in segments" :key="seg.id">
                <label class="font-bold block">{{seg.name}}:</label>

                <DropdownSegment v-if="seg.captureType==='TimeCodeSet'"
                                 v-model="seg.value" :codes="seg.timeCodes" />

                <textarea v-if="seg.captureType==='Memo'" v-model="seg.value"
                          class="mt-1 bg-white border border-grey-700 text-gray-700
                          focus:outline-none focus:border-primary
                          focus:shadow-outline w-full p-2" />

                <div v-if="!seg.valid" class="text-red-500">Required for entry!</div>
            </div>

            <div class="mb-2 flex items-center justify-center">
                <button class="bg-primary text-primary-inv cursor-pointer
                               disabled:opacity-25 disabled:cursor-not-allowed
                               hover:bg-primary-alt
                               px-5 py-1 rounded shadow-md mx-1"
                        :disabled="!valid"
                        @click="book(15)">15 mins</button>
                <button class="bg-primary text-primary-inv cursor-pointer
                               disabled:opacity-25 disabled:cursor-not-allowed
                               hover:bg-primary-alt
                               px-5 py-1 rounded shadow-md mx-1"
                        :disabled="!valid"
                        @click="book(30)">30 mins</button>
                <button class="bg-primary text-primary-inv cursor-pointer
                               disabled:opacity-25 disabled:cursor-not-allowed
                               hover:bg-primary-alt
                               px-5 py-1 rounded shadow-md mx-1"
                        :disabled="!valid"
                        @click="book(60)">1 hour</button>
            </div>

            <div v-if="submitting" class="mb-2 bg-primary text-primary-inv p-5 text-center">
                {{submitting}}
            </div>

            <div v-if="error" class="mb-2 bg-red-600 text-white p-5 text-center">
                ERROR!<br/>
                {{error}}
            </div>

        </template>

    </div>
</template>

<script>
import { markRaw, computed } from "vue";

import agent from "../agents/timeAgent";
import bladeManager from "../../infrastructure/blades";

export default markRaw(
    {
        props: ["workItemId", "reference", "title"],
        data()
        {
            return {
                loadingCategories: true,
                categories: [],
                selectedCategory: null,
                loadingSegments: false,
                segments: [],
                submitting: null,
                error: null
            };
        },
        computed:
        {
            valid()
            {
                var errors = 0;
                this.segments.forEach(seg =>
                {
                    if (!seg.valid) errors++;
                });

                if (this.submitting) errors++;

                return errors <= 0;
            }
        },
        created()
        {
            this.loadCategories();
        },
        methods:
        {
            loadCategories()
            {
                this.loadingCategories = true;

                agent.getCategories().then(cats =>
                {
                    this.categories = cats;
                    this.selectedCategory = cats[0].systemName;
                    this.loadingCategories = false;
                    this.onCategoryChanged();
                });
            },
            onCategoryChanged()
            {
                this.loadingSegments = true;
                agent.getCapture(this.selectedCategory, this.workItemId).then(def =>
                {
                    // Copy into our reactive model
                    this.segments = def.segments;

                    // Enrich reactive state
                    this.segments.forEach(seg =>
                    {
                        seg.value = null;
                        seg.valid = computed(() => !seg.isMandatoryForEntry || seg.value);
                    });

                    this.loadingSegments = false;
                });
            },
            book(mins)
            {
                this.submitting = "Please wait, submitting";

                var segments = this.segments.map(seg =>
                {
                    return {
                        id: seg.id,
                        segmentValue: seg.captureType === "Memo" ? seg.value : null,
                        timeCodeId: seg.captureType === "TimeCodeSet" ? seg.value : null
                    };
                })

                agent.sendTime(this.workItemId, this.selectedCategory, mins, segments).then(
                    () =>
                    {
                        this.submitting = "Successfully sent time... this blade will auto close";

                        window.setTimeout(() =>
                        {
                            bladeManager.closeTopBlade();
                        }, 3000)
                    },
                    err =>
                    {
                        this.error = err;
                    });
            }
        }

    });
</script>