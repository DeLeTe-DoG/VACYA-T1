<template>
  <!-- <div class="chart-block"> -->
  <v-chart :option="chartOptions" class="chart-block" />
  <!-- {{ chartData }} -->
  <!-- </div> -->
</template>

<script>
import "echarts";
import VChart from "vue-echarts";

export default {
  components: { VChart },
  props: {
    chartData: {
      type: Object,
      required: true,
    },
    period: {
      type: Array,
      required: true,
    },
  },
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
              color: "#DAA011",
            },
            name: "API errors",
            // data: [100, 50, 40, 110],
            data: [],
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
              color: "#F62E2E",
            },
            name: "Server errors",
            // data: [105, 52, 20, 78],
            // smooth: true,
          },
        ],
        tooltip: {
          trigger: "axis",
        },
        // title: {
        //   text: "Основные метрики",
        // },
        xAxis: {
          data: ["01-09-25", "02-09-25", "03-09-25", "04-09-25"],
        },
        yAxis: {
          show: true,
          // interval: 20,
        },
      },
      line1: null,
      line2: [],
    };
  },
  methods: {
    sortByDate(data) {
      const errorsByDay = {};
      data.forEach((entry) => {
        console.log(entry.id[0]);
        if (entry.id[0] !== "2") {
          const date = new Date(entry.lastChecked).toISOString().split("T")[0]; // Получаем дату в формате YYYY-MM-DD
          if (!errorsByDay[date]) {
            errorsByDay[date] = 0;
          }
          errorsByDay[date]++;
        }
      });
      const errorsCount = Object.values(errorsByDay);
      console.log(errorsCount);
      return errorsCount;
    },
    extractUniqueDates(data) {
      const uniqueDates = new Set(); // Используем Set для уникальности

      data.forEach((entry) => {
        const date = new Date(entry.lastChecked).toISOString().split("T")[0]; // Извлекаем дату без времени
        uniqueDates.add(date); // Добавляем дату в Set
      });

      // Преобразуем Set в массив
      return Array.from(uniqueDates);
    },
  },
  mounted() {
    console.log(this.chartData);
    this.chartOptions.series[0].data = [...this.sortByDate(this.chartData)];
    console.log(this.chartOptions.series[0].data);
    this.chartOptions.xAxis.data = this.extractUniqueDates(this.chartData);
  },
  watch: {
    // chartData(newData) {
    //   this.sortByDate(newData)
    //   this.extractUniqueDates(newData)
    // }
    // "$route.query.project"() {
    //   this.sortByDate(this.chartData);
    //   this.extractUniqueDates(this.chartData);
    // },
  },
};
</script>

<style lang="scss" scoped>
.chart-block {
  min-height: 500px !important;
  width: 100%;
}
</style>
