<template>
  <!-- <div class="chart-block"> -->
  <v-chart :option="chartOptions" class="chart-block" />
  <!-- </div> -->
</template>

<script>
import "echarts";
import VChart from "vue-echarts";

export default {
  components: { VChart },
  data() {
    return {
      chartOptions: {
        grid: {
          left: 40,
          right: 0,
          top: 80,
        },
        series: [
          {
            type: "line",
            lineStyle: {
              color: '#DAA011',
            },
            name: "API errors",
            data: [100, 50, 40, 110],
            // smooth: true,
            // markPoint: {
            //   data: [
            //     { type: "max", name: "Max" },
            //     { type: "min", name: "Min" },
            //   ],
            // },
            // markLine: {
            //   data: [{ type: "average", name: "Avg" }],
            // },
          },
          {
            type: "line",
            lineStyle: {
              color: '#F62E2E',
            },
            name: "Server errors",
            data: [105, 52, 20, 78],
            // smooth: true,
          },
        ],
        tooltip: {
            trigger: 'axis'
        },
        // title: {
        //   text: "Основные метрики",
        // },
        xAxis: {
          data: ["01-09-25", "02-09-25", "03-09-25", "04-09-25"],
        },
        yAxis: {
          show: true,
          interval: 20,
        },
      },
    };
  },
};
</script>

<style lang="scss" scoped>
    .chart-block{
        min-height: 500px !important;
        width: 100%;
    }
</style>
