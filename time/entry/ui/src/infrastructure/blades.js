import { reactive } from "vue";

const state = reactive(
    {
        bladeStack: []
    });

function closeAllBlades()
{
    return new Promise((resolve, reject) =>
    {
        state.bladeStack = [];
        resolve();
    });
}

function closeTopBlade()
{
    return new Promise((resolve, reject) =>
    {
        var stack = state.bladeStack;
        if( stack.length < 1 )
        {
            resolve();
            return;
        }

        stack.splice(stack.length-1, 1);

        resolve();
    })
}

function openBlade(componentRef, props)
{
    return new Promise((resolve, reject) =>
    {
        var entry = {
            component: componentRef,
            props: props,
            depth: state.bladeStack.length
        };

        state.bladeStack.push(entry);

        resolve(entry);
    });
}

export default {
    state: state,
    closeTopBlade: closeTopBlade,
    closeAllBlades: closeAllBlades,
    openBlade: openBlade
};