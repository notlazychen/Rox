<template>
    <el-row>
        <el-col :span="24">
            <div class="grid-content ep-bg-purple-dark" />
        </el-col>
    </el-row>
    <el-row>
        <el-col :span="12">
            <div class="grid-content ep-bg-purple" />
        </el-col>
        <el-col :span="12">
            <div class="grid-content ep-bg-purple-light" />
        </el-col>
    </el-row>
    <el-row>
        <el-col :span="8">
            <div class="grid-content ep-bg-purple" />
        </el-col>
        <el-col :span="8">
            <div class="grid-content ep-bg-purple-light" />
        </el-col>
        <el-col :span="8">
            <div class="grid-content ep-bg-purple" />
        </el-col>
    </el-row>
    <el-row>
        <el-col :span="6">
            <div class="grid-content ep-bg-purple" />
        </el-col>
        <el-col :span="6">
            <div class="grid-content ep-bg-purple-light" />
        </el-col>
        <el-col :span="6">
            <div class="grid-content ep-bg-purple" />
        </el-col>
        <el-col :span="6">
            <div class="grid-content ep-bg-purple-light" />
        </el-col>
    </el-row>
    <el-row>
        <el-col :span="4">
            <div class="grid-content ep-bg-purple" />
        </el-col>
        <el-col :span="4">
            <div class="grid-content ep-bg-purple-light" />
        </el-col>
        <el-col :span="4">
            <div class="grid-content ep-bg-purple" />
        </el-col>
        <el-col :span="4">
            <div class="grid-content ep-bg-purple-light" />
        </el-col>
        <el-col :span="4">
            <div class="grid-content ep-bg-purple" />
        </el-col>
        <el-col :span="4">
            <div class="grid-content ep-bg-purple-light" />
        </el-col>
    </el-row>
</template>

<style lang="scss">
:root {
    --ep-c-bg-row: #f9fafc;
    --ep-c-bg-purple: #d3dce6;
    --ep-c-bg-purple-dark: #99a9bf;
    --ep-c-bg-purple-light: #e5e9f2;
}

.el-row {
    margin-bottom: 20px;
}

.el-row:last-child {
    margin-bottom: 0;
}

.el-col {
    border-radius: 4px;
}

.grid-content {
    border-radius: 4px;
    min-height: 36px;
}

.ep-bg-purple-dark {
    background-color: var(--ep-c-bg-purple-dark);
}
.ep-bg-purple {
    background-color: var(--ep-c-bg-purple);
}
.ep-bg-purple-light {
    background-color: var(--ep-c-bg-purple-light);
}
</style>
