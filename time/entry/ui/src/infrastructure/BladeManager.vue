
<template>
    <div v-if="currentBlade"
         class="fixed top-0 left-0 right-0 bottom-0 bg-[#00000099]"
         @click="closeAllBlades">

        <div class="fixed top-[10px] right-[10px] bottom-[10px] w-[300px] bg-white rounded sm:w-[600px] xxoverflow-auto"
             @click.stop.prevent="null">
            <div class="text-right">
                <span class="p-2 text-muted text-3xl hover:text-black cursor-pointer"
                      @click.stop.prevent="closeCurrentBlade">
                &times;
                </span>
            </div>
            <component :is="currentBlade.component" v-bind="currentBlade.props"/>
        </div>

    </div>
</template>

<script>
import bladeManager from "./blades";

export default
{
    computed:
    {
        currentBlade()
        {
            var stack = bladeManager.state.bladeStack;
            if( stack.length < 1 ) return null;

            return stack[stack.length-1];
        }
    },
    methods:
    {
        closeAllBlades()
        {
            bladeManager.closeAllBlades();
        },
        closeCurrentBlade()
        {
            bladeManager.closeTopBlade();
        }
    }
}
</script>
