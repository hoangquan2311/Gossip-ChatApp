/** @type {import('tailwindcss').Config} */

console.log("Tailwind config loaded");

module.exports = {
  content: ["./index.html", "./src/**/*.{vue,js,ts,jsx,tsx}"],

  darkMode: "class",

  theme: {
    screens: {
      sm: "640px",
      md: "768px",
      lg: "1024px",
      xl: "1280px",
      "2xl": "1536px",
    },
    extend: {
      colors: {
        neon: "#00ff00",
      },
      spacing: {
        0: "0",
        1: ".125rem",
        2: ".25rem",
        3: ".5rem",
        4: ".75rem",
        5: "1rem",
        6: "1.5rem",
        7: "2rem",
        8: "2.5rem",
        9: "3rem",
        10: "4rem",
        11: "5rem",
        12: "6rem",
        13: "10rem",
      },
      
      fontFamily: {
        sans: ["Open Sans", "sans-serif"],
        display: ["Fredoka"],
      },

      borderRadius: {
        none: "0",
        sm: ".5rem",
        DEFAULT: ".75rem",
        lg: "1.25rem",
        full: "624.9375rem",
      },

      boxShadow: {
        "shadow-sm": "0rem .0625rem .1875rem rgba(5,5,5,0.10)",
        shadow:
          "rgb(145 158 171 / 20%) 0rem .0625rem .1875rem, rgb(145 158 171 / 12%) 0rem .0625rem .125rem -0.25rem",
        "shadow-md":
          "0rem .1875rem .25rem rgba(3,3,3,0.1), 0rem .125rem .25rem rgba(3,3,3,0.1)",
        "shadow-lg":
          "0rem .625rem 1.25rem rgba(3,3,3,0.1), 0rem .1875rem .375rem rgba(3,3,3,0.1)",
        "shadow-xl":
          "0rem .9375rem 1.5625rem rgba(3,3,3,0.1), 0rem .3125rem .625rem rgba(3,3,3,0.1)",
        cxl:
          "0 .125rem .3125rem rgba(193, 202, 255, 0.5), .125rem 0 .3125rem rgba(193, 202, 255, 0.5), -0.125rem 0 .3125rem rgba(193, 202, 255, 0.5), 0 -0.125rem .3125rem rgba(193, 202, 255, 0.5)",
      },

      keyframes: {
        gradient: {
          "0%": {
            "background-position": "0% 0%",
          },

          "50%": {
            "background-position": "0% 100%",
          },

          "100%": {
            "background-position": "0% 0%",
          },
        },
      },

      animation: {
        gradient: "gradient 15s ease infinite",
      },

      opacity: {
        0: "0",
        20: "0.20",
        30: "0.30",
        40: "0.40",
        50: "0.54",
        60: "0.63",
        70: "0.70",
        80: "0.80",
        90: "0.90",
        100: "1",
      },
    },
  },

  plugins: [],
};
