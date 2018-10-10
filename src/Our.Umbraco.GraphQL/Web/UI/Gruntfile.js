
module.exports = function(grunt) {
    require('load-grunt-tasks')(grunt);
    var path = require('path');
  
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        pkgMeta: grunt.file.readJSON('config/meta.json'),
        dest: grunt.option('target') || 'dist',
        basePath: path.join('<%= dest %>', 'App_Plugins', '<%= pkgMeta.name %>'),
  
        watch: {
            options: {
                spawn: false,
                atBegin: true
            },
            js: {
                files: ['source/**/*.js'],
                tasks: ['concat:dist']
            },
            html: {
                files: ['source/**/*.html'],
                tasks: ['copy:html']
            },
            sass: {
                files: ['source/**/*.scss'],
                tasks: ['sass', 'copy:css']
            },
            css: {
                files: ['source/**/*.css'],
                tasks: ['copy:css']
            },
            manifest: {
                files: ['source/package.manifest'],
                tasks: ['copy:manifest']
            }
        },
  
        concat: {
            options: {
                stripBanners: false
            },
            dist: {
                src: [
                    'source/resources/graphql.for.umbraco.api.resource.js',
                    'source/controllers/dashboard.controller.js'
                ],
                dest: '<%= basePath %>/js/graphql.for.umbraco.js'
            }
        },
  
        copy: {
            html: {
                cwd: 'source/views/',
                src: [
                    'dashboard.html'
                ],
                dest: '<%= basePath %>/views/',
                expand: true,
                rename: function(dest, src) {
                    return dest + src;
                }
            },
            css: {
                cwd: 'source/css/',
                src: [
                    'graphql.for.umbraco.css'
                ],
                dest: '<%= basePath %>/css/',
                expand: true,
                rename: function(dest, src) {
                    return dest + src;
                }
            },
            manifest: {
                cwd: 'source/',
                src: [
                    'package.manifest'
                ],
                dest: '<%= basePath %>/',
                expand: true,
                rename: function(dest, src) {
                    return dest + src;
                }
            },
            umbraco: {
                cwd: '<%= dest %>',
                src: '**/*',
                dest: 'tmp/umbraco',
                expand: true
            }
        },
  
        umbracoPackage: {
            dist: {
                src: 'tmp/umbraco',
                dest: 'pkg',
                options: {
                    name: "<%= pkgMeta.name %>",
                    version: '<%= pkgMeta.version %>',
                    url: '<%= pkgMeta.url %>',
                    license: '<%= pkgMeta.license %>',
                    licenseUrl: '<%= pkgMeta.licenseUrl %>',
                    author: '<%= pkgMeta.author %>',
                    authorUrl: '<%= pkgMeta.authorUrl %>',
                    manifest: 'config/package.xml',
                    readme: '<%= grunt.file.read("config/readme.txt") %>',
                }
            }
        },
  
        jshint: {
            options: {
                jshintrc: '.jshintrc'
            },
            src: {
                src: ['app/**/*.js', 'lib/**/*.js']
            }
        },
    
        sass: {
            dist: {
                options: {
                    style: 'compressed'
                },
                files: {
                    'source/css/graphql.for.umbraco.css': 'source/sass/graphql.for.umbraco.scss'
                }
            }
        },
  
        clean: {
            build: '<%= grunt.config("basePath").substring(0, 4) == "dist" ? "dist/**/*" : "null" %>',
            tmp: ['tmp'],
            html: [
                'source/views/*.html',
                '!source/views/dashboard.html'
                ],
            js: [
                'source/controllers/*.js',
                '!source/resources/graphql.for.umbraco.api.resource.js',
                '!source/controllers/dashboard.controller.js'
            ],
            css: [
                'source/css/*.css',
                '!source/css/graphql.for.umbraco.css'
            ],
            sass: [
                'source/sass/*.scss',
                '!source/sass/graphql.for.umbraco.scss'
            ]
        },
    });
  
    grunt.registerTask('default', ['concat', 'sass:dist', 'copy:html', 'copy:manifest', 'copy:css', 'clean:html', 'clean:js', 'clean:css']);
    grunt.registerTask('umbraco', ['clean:tmp', 'default', 'copy:umbraco', 'umbracoPackage', 'clean:tmp']);
  };
  