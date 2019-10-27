/// <binding BeforeBuild='default' />
module.exports = function (grunt) {

    'use strict';
    // Project configuration.
	const sass = require('node-sass');

    grunt.initConfig({

        pkg: grunt.file.readJSON('package-lock.json'),

        // Sass

        sass: {

            options: {

                sourceMap: true, // Create source map
				implementation: sass,
                outputStyle: 'compressed' // Minify output

            },

            dist: {

                files: [

                    {

                        expand: true, // Recursive

                        cwd: "wwwroot/sass", // The startup directory

                        src: ["**/*.scss"], // Source files

                        dest: "wwwroot/css", // Destination

                        ext: ".css" // File extension

                    }

                ]

            }

        }

    });

    // Load the plugin

    grunt.loadNpmTasks('grunt-sass');

    // Default task(s).

    grunt.registerTask('default', ['sass']);

};