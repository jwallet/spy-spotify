=begin

Jekyll  Multiple  Languages  is  an  internationalization  plugin for Jekyll. It
compiles  your  Jekyll site for one or more languages with a similar approach as
Rails does. The different sites will be stored in sub folders with the same name
as the language it contains.

Please visit https://github.com/screeninteraction/jekyll-multiple-languages-plugin
for more details.

=end



require_relative "plugin/version"

module Jekyll

  #*****************************************************************************
  # :site, :post_render hook
  #*****************************************************************************
  Jekyll::Hooks.register :site, :pre_render do |site, payload|
      lang = site.config['lang']
      puts "Loading translation from file #{site.source}/_i18n/#{lang}.yml"
      site.parsed_translations[lang] = YAML.load_file("#{site.source}/_i18n/#{lang}.yml")
  end
  Jekyll::Hooks.register :site, :post_render do |site, payload|
    
    # Removes all static files that should not be copied to translated sites.
    #===========================================================================
    default_lang  = payload["site"]["default_lang"]
    current_lang  = payload["site"][        "lang"]
    
    static_files  = payload["site"]["static_files"]
    exclude_paths = payload["site"]["exclude_from_localizations"]
    
    
    if default_lang != current_lang
      static_files.delete_if do |static_file|
          
        # Remove "/" from beginning of static file relative path
        if static_file.instance_variable_get(:@relative_path) != nil
          static_file_r_path = static_file.instance_variable_get(:@relative_path).dup
          if static_file_r_path
            static_file_r_path[0] = ''

            exclude_paths.any? do |exclude_path|
              Pathname.new(static_file_r_path).descend do |static_file_path|
                break(true) if (Pathname.new(exclude_path) <=> static_file_path) == 0
              end
            end
          end
        end
      end
    end
    
    #===========================================================================
    
  end



  ##############################################################################
  # class Site
  ##############################################################################
  class Site
    
    attr_accessor :parsed_translations   # Hash that stores parsed translations read from YAML files.
    
    alias :process_org :process
    
    #======================================
    # process
    #
    # Reads Jekyll and plugin configuration parameters set on _config.yml, sets
    # main parameters and processes the website for each language.
    #======================================
    def process
      # Check if plugin settings are set, if not, set a default or quit.
      #-------------------------------------------------------------------------
      self.parsed_translations ||= {}
      
      self.config['exclude_from_localizations'] ||= []
      
      if ( !self.config['languages']         or
            self.config['languages'].empty?  or
           !self.config['languages'].all?
         )
          puts 'You must provide at least one language using the "languages" setting on your _config.yml.'
          
          exit
      end
      
      
      # Variables
      #-------------------------------------------------------------------------
      
      # Original Jekyll configurations
      baseurl_org                 = self.config[ 'baseurl' ].to_s # Baseurl set on _config.yml
      dest_org                    = self.dest                     # Destination folder where the website is generated
      
      # Site building only variables
      languages                   = self.config['languages'] # List of languages set on _config.yml
      
      # Site wide plugin configurations
      self.config['default_lang'] = languages.first          # Default language (first language of array set on _config.yml)
      self.config[        'lang'] = languages.first          # Current language being processed
      self.config['baseurl_root'] = baseurl_org              # Baseurl of website root (without the appended language code)
      self.config['translations'] = self.parsed_translations # Hash that stores parsed translations read from YAML files. Exposes this hash to Liquid.
      
      
      # Build the website for default language
      #-------------------------------------------------------------------------
      puts "Building site for default language: \"#{self.config['lang']}\" to: #{self.dest}"
      
      process_org
      
      
      # Build the website for the other languages
      #-------------------------------------------------------------------------
      
      # Remove .htaccess file from included files, so it wont show up on translations folders.
      self.include -= [".htaccess"]
      
      languages.drop(1).each do |lang|
        
        # Language specific config/variables
        @dest                  = dest_org    + "/" + lang
        self.config['baseurl'] = baseurl_org + "/" + lang
        self.config['lang']    =                     lang
        
        puts "Building site for language: \"#{self.config['lang']}\" to: #{self.dest}"
        
        process_org
      end
      
      # Revert to initial Jekyll configurations (necessary for regeneration)
      self.config[ 'baseurl' ] = baseurl_org  # Baseurl set on _config.yml
      @dest                    = dest_org     # Destination folder where the website is generated
      
      puts 'Build complete'
    end



    if Gem::Version.new(Jekyll::VERSION) < Gem::Version.new("3.0.0")
      alias :read_posts_org :read_posts

      #======================================
      # read_posts
      #======================================
      def read_posts(dir)
        translate_posts = !self.config['exclude_from_localizations'].include?("_posts")
        
        if dir == '' && translate_posts
          read_posts("_i18n/#{self.config['lang']}/")
        else
          read_posts_org(dir)
        end
        
      end
    end

  end



  ##############################################################################
  # class PostReader
  ##############################################################################
  class PostReader
  
    if Gem::Version.new(Jekyll::VERSION) >= Gem::Version.new("3.0.0")
      alias :read_posts_org :read_posts
      
      #======================================
      # read_posts
      #======================================
      def read_posts(dir)
        translate_posts = !site.config['exclude_from_localizations'].include?("_posts")
        if dir == '' && translate_posts
          read_posts("_i18n/#{site.config['lang']}/")
        else
          read_posts_org(dir)
        end
      end
    end
  end
  
  
  
  #-----------------------------------------------------------------------------
  #
  # Include (with priority—prepend)the translated
  # permanent link for Page and document
  #
  #-----------------------------------------------------------------------------

  module Permalink
    #======================================
    # permalink
    #======================================
    def permalink
      return nil if data.nil? || data['permalink'].nil?
      
      if site.config['relative_permalinks']
        File.join(@dir,  data['permalink'])
      else
        # Look if there's a permalink overwrite specified for this lang
        data['permalink_'+site.config['lang']] || data['permalink']
      end
      
    end
  end

  Page.prepend(Permalink)
  Document.prepend(Permalink)


  ##############################################################################
  # class Post
  ##############################################################################
  class Post
  
    if Gem::Version.new(Jekyll::VERSION) < Gem::Version.new("3.0.0")
      alias :populate_categories_org :populate_categories
      
      #======================================
      # populate_categories
      #
      # Monkey patched this method to remove unwanted strings
      # ("_i18n" and language code) that are prepended to posts categories
      # because of how the multilingual posts are arranged in subfolders.
      #======================================
      def populate_categories
        categories_from_data = Utils.pluralized_array_from_hash(data, 'category', 'categories')
        self.categories = (
          Array(categories) + categories_from_data
        ).map {|c| c.to_s.downcase}.flatten.uniq
        
        self.categories.delete("_i18n")
        self.categories.delete(site.config['lang'])
        
        return self.categories
      end
    end
  end



  ##############################################################################
  # class Document
  ##############################################################################
  class Document
    
    if Gem::Version.new(Jekyll::VERSION) >= Gem::Version.new("3.0.0")
      alias :populate_categories_org :populate_categories
      
      #======================================
      # populate_categories
      #
      # Monkey patched this method to remove unwanted strings
      # ("_i18n" and language code) that are prepended to posts categories
      # because of how the multilingual posts are arranged in subfolders.
      #======================================
      def populate_categories
        data['categories'].delete("_i18n")
        data['categories'].delete(site.config['lang'])
        
        merge_data!({
          'categories' => (
            Array(data['categories']) + Utils.pluralized_array_from_hash(data, 'category', 'categories')
          ).map(&:to_s).flatten.uniq
        })
      end
    end
  end
  
  
  
  #-----------------------------------------------------------------------------
  #
  # The next classes implements the plugin Liquid Tags and/or Filters
  #
  #-----------------------------------------------------------------------------


  ##############################################################################
  # class LocalizeTag
  #
  # Localization by getting localized text from YAML files.
  # User must use the "t" or "translate" liquid tags.
  ##############################################################################
  class LocalizeTag < Liquid::Tag
  
    #======================================
    # initialize
    #======================================
    def initialize(tag_name, key, tokens)
      super
      @key = key.strip
    end
    
    
    
    #======================================
    # render
    #======================================
    def render(context)
      if      "#{context[@key]}" != "" # Check for page variable
        key = "#{context[@key]}"
      else
        key =            @key
      end
      
      key = Liquid::Template.parse(key).render(context)  # Parses and renders some Liquid syntax on arguments (allows expansions)
      
      site = context.registers[:site] # Jekyll site object
      
      lang = site.config['lang']
      
      translation = site.parsed_translations[lang].access(key) if key.is_a?(String)
      
      if translation.nil? or translation.empty?
         translation = site.parsed_translations[site.config['default_lang']].access(key)
        
        puts "Missing i18n key: #{lang}:#{key}"
        puts "Using translation '%s' from default language: %s" %[translation, site.config['default_lang']]
      end
      
      translation
    end
  end



  ##############################################################################
  # class LocalizeInclude
  #
  # Localization by including whole files that contain the localization text.
  # User must use the "tf" or "translate_file" liquid tags.
  ##############################################################################
  module Tags
    class LocalizeInclude < IncludeTag
    
      #======================================
      # render
      #======================================
      def render(context)
        if       "#{context[@file]}" != "" # Check for page variable
          file = "#{context[@file]}"
        else
          file =            @file
        end
        
        file = Liquid::Template.parse(file).render(context)  # Parses and renders some Liquid syntax on arguments (allows expansions)
        
        site = context.registers[:site] # Jekyll site object
        
        default_lang = site.config['default_lang']

        validate_file_name(file)

        includes_dir = File.join(site.source, '_i18n/' + site.config['lang'])

        # If directory doesn't exist, go to default lang
        if !Dir.exist?(includes_dir)
          includes_dir = File.join(site.source, '_i18n/' + default_lang)
        elsif
          # If file doesn't exist, go to default lang
          Dir.chdir(includes_dir) do
            choices = Dir['**/*'].reject { |x| File.symlink?(x) }
            if !choices.include?(  file)
              includes_dir = File.join(site.source, '_i18n/' + default_lang)
            end
          end
        end
        
        Dir.chdir(includes_dir) do
          choices = Dir['**/*'].reject { |x| File.symlink?(x) }
          
          if choices.include?(  file)
            source  = File.read(file)
            partial = Liquid::Template.parse(source)
            
            context.stack do
              context['include'] = parse_params(  context) if @params
              contents           = partial.render(context)
              ext                = File.extname(file)
              
              converter = site.converters.find { |c| c.matches(ext) }
              contents  = converter.convert(contents) unless converter.nil?
              
              contents
            end
          else
            raise IOError.new "Included file '#{file}' not found in #{includes_dir} directory"
          end
          
        end
      end
    end
  end



  ##############################################################################
  # class LocalizeLink
  #
  # Creates links or permalinks for translated pages.
  # User must use the "tl" or "translate_link" liquid tags.
  ##############################################################################
  class LocalizeLink < Liquid::Tag

    #======================================
    # initialize
    #======================================
    def initialize(tag_name, key, tokens)
      super
      @key = key
    end
    
    
    
    #======================================
    # render
    #======================================
    def render(context)
      if      "#{context[@key]}" != "" # Check for page variable
        key = "#{context[@key]}"
      else
        key = @key
      end
      
      key = Liquid::Template.parse(key).render(context)  # Parses and renders some Liquid syntax on arguments (allows expansions)
      
      site = context.registers[:site] # Jekyll site object
      
      key          = key.split
      namespace    = key[0]
      lang         = key[1] || site.config[        'lang']
      default_lang =           site.config['default_lang']
      baseurl      =           site.baseurl
      pages        =           site.pages
      url          = "";
      
      if default_lang != lang
        baseurl = baseurl + "/" + lang
      end
      
      collections = site.collections.values.collect{|x| x.docs}.flatten
      pages = site.pages + collections
      
      for p in pages
        unless             p['namespace'].nil?
          page_namespace = p['namespace']
          
          if namespace == page_namespace
            permalink = p['permalink_'+lang] || p['permalink']
            url       = baseurl + permalink
          end
        end
      end
      
      url
    end
  end
  
  
end # End module Jekyll



################################################################################
# class Hash
################################################################################
unless Hash.method_defined? :access
  class Hash
  
    #======================================
    # access
    #======================================
    def access(path)
      ret = self
      
      path.split('.').each do |p|
      
        if p.to_i.to_s == p
          ret = ret[p.to_i]
        else
          ret = ret[p.to_s] || ret[p.to_sym]
        end
        
        break unless ret
      end
      
      ret
    end
  end
end



################################################################################
# Liquid tags definitions

Liquid::Template.register_tag('t',              Jekyll::LocalizeTag          )
Liquid::Template.register_tag('translate',      Jekyll::LocalizeTag          )
Liquid::Template.register_tag('tf',             Jekyll::Tags::LocalizeInclude)
Liquid::Template.register_tag('translate_file', Jekyll::Tags::LocalizeInclude)
Liquid::Template.register_tag('tl',             Jekyll::LocalizeLink         )
Liquid::Template.register_tag('translate_link', Jekyll::LocalizeLink         )
