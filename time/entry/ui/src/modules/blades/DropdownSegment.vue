<template>
    <div>
        <select class="mt-1 bg-white border border-grey-700 text-gray-700
                   focus:outline-none focus:border-primary
                   focus:shadow-outline w-full p-2"
                v-model="currentSelection" @change="onChange">
            <option v-for="c in codes" :key="c.id" :value="c">{{c.name}} : {{c.code}}</option>
        </select>
        <DropdownSegment v-if="recurse" :modelValue="modelValue" @update:modelValue="passUp"
                         :codes="currentSelection.children"
                         />
    </div>
</template>

<script>
export default
    {
        props: ["modelValue", "codes"],
        emits: ["update:modelValue"],
        data()
        {
            return {
                currentSelection: null
            };
        },
        computed:
        {
            recurse()
            {
                if (!this.currentSelection) return false;
                if (!this.currentSelection.children || !this.currentSelection.children.length) return false;
                return true;
            }
        },
        updated()
        {
            // modelValue or codes may have been updates with the existing mounted component
            // so check the current selection is still valid and if not, blat it.
            var current = this.currentSelection;
            if( !current ) return;

            var validSelection = this.codes.find(c => c === current);
            if( !validSelection )
            {
                this.currentSelection = null;
            }
        },
        methods:
        {
            passUp(newId)
            {
                this.$emit("update:modelValue", newId);
            },
            onChange()
            {
                // Selection at this level changed - if we're a leaf node, select it,
                // otherwise, select null as the main value.
                var selected = this.currentSelection;
                if( !selected || (selected.children && selected.children.length) )
                {
                    this.$emit("update:modelValue", null);
                }
                else
                {
                    this.$emit("update:modelValue", selected.id);
                }
            }
        }

    };
</script>